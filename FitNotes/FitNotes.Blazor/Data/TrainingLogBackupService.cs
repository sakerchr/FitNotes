using FitNotes.Core.FitNotesBackup;
using FitNotes.Core.Models;

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
    }
}