using Azure.Identity;
using FitNotes.Core;
using FitNotes.Core.FitNotesBackup;

var credential = new DefaultAzureCredential();
var storageAccount = "yourstorageaccount";
var blobContainer = "yourblobcontainer";
var fitnotesBackup = "nameofyourbackupblob";
var trainingLogTable = "nameofyourtraininglogtable";
var exerciseTable = "nameofyourexercisetable";
var exerciseCategoryTable = "nameofyourexercisecategorytable";


var downloader = new BackupDownloader();
var filepath = await downloader.DownloadBlobAsync(storageAccount, blobContainer, fitnotesBackup, credential);
var backup = await BackupParser.ParseFitNotesBackupFile($"Data source={filepath}");
await Task.WhenAll(new List<Task> {
    BackupUploader.InsertTrainingLogs(storageAccount, trainingLogTable, backup.TrainingLogs, credential),
    BackupUploader.InsertExercises(storageAccount, exerciseTable, backup.Exercises, credential),
    BackupUploader.InsertExerciseCategories(storageAccount, exerciseCategoryTable, backup.Categories, credential)
});

var trainingLogClient = new TrainingLogClient(storageAccount);
var prs = await trainingLogClient.GetPersonalRecords(trainingLogTable, credential, orderedAscending: false);