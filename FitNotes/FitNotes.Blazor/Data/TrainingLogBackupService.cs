using FitNotes.Core.FitNotesBackup;
using FitNotes.Core.Models;

namespace FitNotes.Blazor.Data
{
    public class TrainingLogBackupService
    {
        private static readonly string BackupFilePath = $"Data Source={Directory.GetCurrentDirectory()}/Data/backup.sqlite";
        private TrainingLogBackup? _trainingLogBackup { get; set; }

        public async Task<TrainingLogBackup> GetTrainingLogBackupAsync()
        {
            if (_trainingLogBackup is not null) return _trainingLogBackup;
            var backup = await DownloadDataAsync();
            _trainingLogBackup = backup;
            return backup;
        }

        private static async Task<TrainingLogBackup> DownloadDataAsync() => await BackupParser.ParseFitNotesBackupFile(BackupFilePath);
    }
}