using Azure.Core;
using Azure.Storage.Blobs;

namespace FitNotes.Core.FitNotesBackup
{
    public class BackupDownloader
    {
        public readonly string LocalBlobFilePathPrefix;

        public BackupDownloader()
        {
            LocalBlobFilePathPrefix = $"{Directory.GetCurrentDirectory()}\\blobs";
        }

        public async Task<string> DownloadBlobAsync(string storageAccount, string container, string blob, TokenCredential credential)
        {
            var blobClient = new BlobClient(new Uri($"https://{storageAccount}.blob.core.windows.net/{container}/{blob}"), credential);

            Directory.CreateDirectory(LocalBlobFilePathPrefix);
            var localBlobPath = Path.Combine(LocalBlobFilePathPrefix, blob);

            await blobClient.DownloadToAsync(localBlobPath);

            return localBlobPath;
        }
    }
}
