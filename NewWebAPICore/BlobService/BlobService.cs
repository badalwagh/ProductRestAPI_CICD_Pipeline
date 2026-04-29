using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace NewWebAPICore.Service
{
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(string connectionString, string containername)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Azure Blob Storage connection string is required");

            var blobServiceClient = new BlobServiceClient(connectionString);
            
            _containerClient = blobServiceClient.GetBlobContainerClient(containername);
        }

        // Upload file to blob storage
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            try
            {
                string blobName = $"{Guid.NewGuid()}_{file.FileName}";
                var blobClient = _containerClient.GetBlobClient(blobName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file to blob storage: {ex.Message}");
            }
        }

        // Download file from blob storage
        public async Task<Stream> DownloadFileAsync(string blobName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(blobName);
                var download = await blobClient.DownloadAsync();
                return download.Value.Content;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading file from blob storage: {ex.Message}");
            }
        }

        // Delete file from blob storage
        public async Task<bool> DeleteFileAsync(string blobName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(blobName);
                await blobClient.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file from blob storage: {ex.Message}");
            }
        }
    }
}
