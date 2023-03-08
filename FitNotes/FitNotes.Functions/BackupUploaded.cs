using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using FitNotes.Core.FitNotesBackup;
using FitNotes.Core.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FitNotes.Functions
{
    public class BackupUploaded
    {
        [FunctionName(nameof(BackupUploaded))]
        public async Task Run([BlobTrigger("fitnotesbackups/{name}", Connection = "StorageAccountConnectionString")]Stream myBlob, string name, Uri uri, ILogger log, [DurableClient] IDurableOrchestrationClient client)
        {
            log.LogInformation("Initiating download, parsing and upload of backup stored in blob {FileName} to Table storage", name);
            var instanceId = await client.StartNewAsync(nameof(ProcessTrainingLogBackup), new BackupUploadedPayload(uri, "fitnoteswithtables", "fitnoteswithtables", name));
        }

        [FunctionName(nameof(ProcessTrainingLogBackup))]
        public async Task ProcessTrainingLogBackup([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var payload = context.GetInput<BackupUploadedPayload>();
            var trainingLogBackup = await context.CallActivityAsync<TrainingLogBackup>(nameof(DownloadAndParseBackupFromStream), payload);

            await context.CallSubOrchestratorAsync(nameof(UploadBackupToTableStorage), new UploadBackupToTableStoragePayload(payload.StorageAccount, payload.Table, trainingLogBackup));
            log.LogInformation("Successfully downloaded, parsed and uploaded training log backup stored in blob {FileName}", payload.BlobName);
        }

        [FunctionName(nameof(UploadBackupToTableStorage))]
        public async Task UploadBackupToTableStorage([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var payload = context.GetInput<UploadBackupToTableStoragePayload>();
            var tasksToComplete = new Task[] {
                context.CallActivityAsync(nameof(UploadBackupTrainingLogsToTableStorage), new BackupTrainingLogsPayload(payload.StorageAccount, "TrainingLog", payload.TrainingLogBackup.TrainingLogs)),
                context.CallActivityAsync(nameof(UploadBackupExercisesToTableStorage), new BackupExercisesPayload(payload.StorageAccount, "Exercise", payload.TrainingLogBackup.Exercises)),
                context.CallActivityAsync(nameof(UploadBackupCategoriesToTableStorage), new BackupCategoriesPayload(payload.StorageAccount, "Category", payload.TrainingLogBackup.Categories))
            };

            log.LogInformation("Uploading a total of {NumberOfTrainingLogs} training logs to Table storage", payload.TrainingLogBackup.TrainingLogs.Count);
            await Task.WhenAll(tasksToComplete);
            log.LogInformation("Successfully uploaded {NumberOfTrainingLogs} training logs to Table storage", payload.TrainingLogBackup.TrainingLogs.Count);
        }

        [FunctionName(nameof(DownloadAndParseBackupFromStream))]
        public async Task<TrainingLogBackup> DownloadAndParseBackupFromStream([ActivityTrigger] BackupUploadedPayload payload, ILogger log)
        {
            log.LogInformation("Downloading and parsing blob {FileName}", payload.BlobName);
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".fitnotes");

            var blobClient = new BlobClient(payload.BlobUri, new DefaultAzureCredential());
            using (var memoryStream = new MemoryStream())
            {
                blobClient.OpenRead().CopyTo(memoryStream);
                File.WriteAllBytes(filePath, memoryStream.ToArray());
            }

            var parsedBackup = await BackupParser.ParseFitNotesBackupFile($"Data source={filePath}");
            log.LogInformation("Successfully read and parsed blob {FileName} which contained {NumberOfTrainingLogs} training log entries", payload.BlobName, parsedBackup.TrainingLogs.Count);

            return parsedBackup;
        }

        [FunctionName(nameof(UploadBackupTrainingLogsToTableStorage))]
        public async Task UploadBackupTrainingLogsToTableStorage([ActivityTrigger] BackupTrainingLogsPayload payload) => 
            await BackupUploader.InsertTrainingLogs(payload.StorageAccount, payload.Table, payload.TrainingLogs, new DefaultAzureCredential());

        [FunctionName(nameof(UploadBackupExercisesToTableStorage))]
        public async Task UploadBackupExercisesToTableStorage([ActivityTrigger] BackupExercisesPayload payload) => 
            await BackupUploader.InsertExercises(payload.StorageAccount, payload.Table, payload.Exercises, new DefaultAzureCredential());

        [FunctionName(nameof(UploadBackupCategoriesToTableStorage))]
        public async Task UploadBackupCategoriesToTableStorage([ActivityTrigger] BackupCategoriesPayload payload) =>
            await BackupUploader.InsertExerciseCategories(payload.StorageAccount, payload.Table, payload.Categories, new DefaultAzureCredential());

    }

    public record BackupUploadedPayload(Uri BlobUri, string StorageAccount, string Table, string BlobName);
    public record UploadBackupToTableStoragePayload(string StorageAccount, string Table, TrainingLogBackup TrainingLogBackup);
    public record BackupExercisesPayload(string StorageAccount, string Table, List<Exercise> Exercises);
    public record BackupCategoriesPayload(string StorageAccount, string Table, List<Category> Categories);
    public record BackupTrainingLogsPayload(string StorageAccount, string Table, List<TrainingLog> TrainingLogs);
}
