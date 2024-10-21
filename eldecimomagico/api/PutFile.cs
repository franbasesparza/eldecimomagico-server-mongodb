using System;
using System.IO;
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
    public static class PutFile
    {
        private static string storageAccountName = Environment.GetEnvironmentVariable("PhotoStorageAccountName");
        private static string storageAccountKey = Environment.GetEnvironmentVariable("PhotoStorageAccountKey");

        [FunctionName("PutFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "putfile/{containerName}/{blobName}/{format}")] HttpRequest req,
            ILogger log,
            string containerName,
            string blobName,
            string format)
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

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                byte[] contentBytes = Convert.FromBase64String(requestBody);

                using (var stream = new MemoryStream(contentBytes))
                {
                    await blockBlob.UploadFromStreamAsync(stream);
                }

                // Set content type as image
                blockBlob.Properties.ContentType = $"image/{format}";
                await blockBlob.SetPropertiesAsync();

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(
                    ex,
                    $"Error putting file: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );
                return new StatusCodeResult(500);
            }
        }
    }
}
