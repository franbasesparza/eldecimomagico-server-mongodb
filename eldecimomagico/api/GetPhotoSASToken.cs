using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;

namespace eldecimomagico.api
{
    public static class GetPhotoSASToken
    {
        private static string storageAccountName = Environment.GetEnvironmentVariable("PhotoStorageAccountName");
        private static string storageAccountKey = Environment.GetEnvironmentVariable("PhotoStorageAccountKey");
        
        [FunctionName("GetPhotoSASToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getphotosastoken/{containerName}")] HttpRequest req,
            ILogger log,
            string containerName)
        {
            try
            {
                CloudStorageAccount storageAccount = new CloudStorageAccount(
                    new Microsoft.Azure.Storage.Auth.StorageCredentials(
                        storageAccountName,
                        storageAccountKey
                    ),
                    true
                );

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                // Set the expiry time and permissions for the SAS token
                SharedAccessBlobPolicy sasPolicy = new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Write, // Permissions for the SAS token (e.g., Write, Read, etc.)
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1), // Expiry time for the SAS token
                };

                // Generate the SAS token
                string sasToken = container.GetSharedAccessSignature(sasPolicy);

                string sasUrl = container.Uri + sasToken;

                var jsonData = JsonConvert.SerializeObject(sasUrl);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error generating SAS token");
                return new StatusCodeResult(500);
            }
        }
    }
}
