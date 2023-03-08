using Azure.Core;
using Azure.Data.Tables;
using FitNotes.Core.Models.TableEntities;

namespace FitNotes.Core
{
    public class TrainingLogClient
    {
        private readonly Uri StorageAccountEndpoint;

        public TrainingLogClient(string storageAccount)
        {
            StorageAccountEndpoint = new Uri($"https://{storageAccount}.table.core.windows.net/");
        }

        public async Task<List<TrainingLogTableEntity>> GetTrainingLogs(string table, TokenCredential credential, int numberOfMonths = 12, bool orderedAscending = true)
        {
            var tableClient = new TableClient(StorageAccountEndpoint, table, credential);
            var cutoffDate = DateTime.UtcNow.AddMonths(-numberOfMonths);
            var logsToReturn = new List<TrainingLogTableEntity>();

            await foreach (var trainingLogs in tableClient.QueryAsync<TrainingLogTableEntity>(tlte => tlte.Date >= cutoffDate))
            {
                logsToReturn.Add(trainingLogs);
            }

            return orderedAscending ? logsToReturn.OrderBy(l => l.Date).ToList() : logsToReturn.OrderByDescending(l => l.Date).ToList();
        }

        public async Task<List<TrainingLogTableEntity>> GetTrainingLogsByExercise(string table, TokenCredential credential, string exercise, int numberOfMonths = 12, bool orderedAscending = true)
        {
            var tableClient = new TableClient(StorageAccountEndpoint, table, credential);
            var cutoffDate = DateTime.UtcNow.AddMonths(-numberOfMonths);
            var logsToReturn = new List<TrainingLogTableEntity>();

            await foreach (var trainingLogs in tableClient.QueryAsync<TrainingLogTableEntity>(tlte => tlte.Exercise == exercise && tlte.Date >= cutoffDate))
            {
                logsToReturn.Add(trainingLogs);
            }

            return orderedAscending ? logsToReturn.OrderBy(l => l.Date).ToList() : logsToReturn.OrderByDescending(l => l.Date).ToList();
        }

        public async Task<List<TrainingLogTableEntity>> GetTrainingLogsByExerciseCategory(string table, TokenCredential credential, string exerciseCategory, int numberOfMonths = 12, bool orderedAscending = true)
        {
            var tableClient = new TableClient(StorageAccountEndpoint, table, credential);
            var cutoffDate = DateTime.UtcNow.AddMonths(-numberOfMonths);
            var logsToReturn = new List<TrainingLogTableEntity>();

            await foreach (var trainingLogs in tableClient.QueryAsync<TrainingLogTableEntity>(tlte => tlte.Category == exerciseCategory && tlte.IsPersonalRecord))
            {
                logsToReturn.Add(trainingLogs);
            }

            return orderedAscending ? logsToReturn.OrderBy(l => l.Date).ToList() : logsToReturn.OrderByDescending(l => l.Date).ToList();
        }

        public async Task<List<TrainingLogTableEntity>> GetPersonalRecords(string table, TokenCredential credential, bool orderedAscending = true)
        {
            var tableClient = new TableClient(StorageAccountEndpoint, table, credential);
            var logsToReturn = new List<TrainingLogTableEntity>();

            await foreach (var trainingLogs in tableClient.QueryAsync<TrainingLogTableEntity>(tlte =>  tlte.IsPersonalRecord))
            {
                logsToReturn.Add(trainingLogs);
            }

            return orderedAscending ? logsToReturn.OrderBy(l => l.Date).ToList() : logsToReturn.OrderByDescending(l => l.Date).ToList();
        }

        public async Task<List<TrainingLogTableEntity>> GetPersonalRecordsByExercise(string table, TokenCredential credential, string exercise, bool orderedAscending = true)
        {
            var tableClient = new TableClient(StorageAccountEndpoint, table, credential);
            var logsToReturn = new List<TrainingLogTableEntity>();

            await foreach (var trainingLogs in tableClient.QueryAsync<TrainingLogTableEntity>(tlte => tlte.Exercise == exercise))
            {
                logsToReturn.Add(trainingLogs);
            }

            return orderedAscending ? logsToReturn.OrderBy(l => l.Date).ToList() : logsToReturn.OrderByDescending(l => l.Date).ToList();
        }
    }
}
