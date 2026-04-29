using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace NewWebAPICore.Service
{
    public class BlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobService(IConfiguration config)
        {
            _connectionString = config["AzureBlobStorage:ConnectionString"];
            _containerName = config["AzureBlobStorage:ContainerName"];
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var blobContainerClient = new BlobContainerClient(_connectionString, _containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }
    }
}
