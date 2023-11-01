using FitNotes.Core.FitNotesBackup;
using FitNotes.Core.Models;

namespace FitNotes.Blazor.Data
{
    public class TrainingLogBackupService
    {
        private readonly ILogger<TrainingLogBackupService> _logger;
        public TrainingLogBackup? TrainingLogBackup { get; set; }

        public TrainingLogBackupService(ILogger<TrainingLogBackupService> logger) => _logger = logger;

        public async Task DownloadData()
        {
            try
            {
                var parsedFile = await BackupParser.ParseFitNotesBackupFile($"Data Source={Directory.GetCurrentDirectory()}/Data/backup.sqlite");
            }
            catch (Microsoft.Data.Sqlite.SqliteException e)
            {
                _logger.LogError("Unable to download data");
            }
        }
    }
}