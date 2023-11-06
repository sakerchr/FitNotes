using FitNotes.Core.FitNotesBackup;
using FitNotes.Core.Models;
using Microsoft.VisualBasic;

namespace FitNotes.Blazor.Data
{
    public class TrainingLogBackupService
    {
        private static readonly string BackupFilePath = $"Data Source={Directory.GetCurrentDirectory()}/Data/backup.sqlite";
        private readonly ILogger<TrainingLogBackupService> _logger;
        private TrainingLogBackup? _trainingLogBackup { get; set; }

        public TrainingLogBackupService(ILogger<TrainingLogBackupService> logger) => _logger = logger;

        public async Task<TrainingLogBackup> GetTrainingLogBackupAsync()
        {
            if (_trainingLogBackup is not null) return _trainingLogBackup;
            var backup = await DownloadDataAsync();
            _logger.LogDebug("Downloading data");
            _trainingLogBackup = backup;
            return backup;
        }

        private static async Task<TrainingLogBackup> DownloadDataAsync() => await BackupParser.ParseFitNotesBackupFile(BackupFilePath);
        public bool TryGetTrainingLogSession(DateOnly date, out TrainingLogSession? trainingLogSession)
        {
            trainingLogSession = null;
            if (_trainingLogBackup is not null)
            {
                var session = _trainingLogBackup.TrainingSessions.FirstOrDefault(ts => ts.Date == date);
                if (session is not null)
                {
                    trainingLogSession = session;
                    return true;
                }
            }
            return false;
        }
    }
}