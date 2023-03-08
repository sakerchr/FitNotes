using Azure.Core;
using Azure.Data.Tables;
using FitNotes.Core.Models;

namespace FitNotes.Core.FitNotesBackup
{
    public class BackupUploader
    {
        public static async Task InsertTrainingLogs(string storageAccount, string table, List<TrainingLog> trainingLogs, TokenCredential credential)
        {
            var tableClient = new TableClient(new Uri($"https://{storageAccount}.table.core.windows.net/"), table, credential);
            var upsertTasks = new List<Task>();

            var trainingLogsGroupedByCategory = trainingLogs.GroupBy(x => x.Category).ToList();
            foreach (var trainingLogCategoryGrouping in trainingLogsGroupedByCategory)
            {
                var trainingLogsBatch = trainingLogCategoryGrouping
                    .Select(tlcg => new TableTransactionAction(
                        TableTransactionActionType.UpsertReplace,
                        tlcg.ToTableEntity()))
                    .ToList();

                for (var i = 0; i < trainingLogsBatch.Count; i += 100)
                {
                    upsertTasks.Add(tableClient.SubmitTransactionAsync(trainingLogsBatch.GetRange(i, Math.Min(100, trainingLogsBatch.Count - i))));
                }
            }

            await Task.WhenAll(upsertTasks);
        }

        public static async Task InsertExerciseCategories(string storageAccount, string table, List<Category> categories, TokenCredential credential)
        {
            var tableClient = new TableClient(new Uri($"https://{storageAccount}.table.core.windows.net/"), table, credential);

            var upsertTasks = new List<Task>();
            foreach (var category in categories)
            {
                upsertTasks.Add(tableClient.UpsertEntityAsync(category.ToTableEntity()));
            }

            await Task.WhenAll(upsertTasks);
        }

        public static async Task InsertExercises(string storageAccount, string table, List<Exercise> exercises, TokenCredential credential)
        {
            var tableClient = new TableClient(new Uri($"https://{storageAccount}.table.core.windows.net/"), table, credential);

            var upsertTasks = new List<Task>();
            foreach (var exercise in exercises)
            {
                upsertTasks.Add(tableClient.UpsertEntityAsync(exercise.ToTableEntity()));
            }

            await Task.WhenAll(upsertTasks);
        }
    }
}
