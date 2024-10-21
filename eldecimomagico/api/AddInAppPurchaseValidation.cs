using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using eldecimomagico.dal;
using MongoDB.Bson;

namespace eldecimomagico.api
{
    public static class AddInAppPurchaseValidation
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("AddInAppPurchaseValidation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "addinapppurchasevalidation")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (AddInAppPurchaseValidation)");

            try
            {
                // Read the request body
                string requestBody = await req.ReadAsStringAsync();
                log.LogInformation("Request Body: {RequestBody}", requestBody); 
                
                var purchaseUpdateWebhook = JsonConvert.DeserializeObject<PurchasesUpdatedWebhook>(requestBody);

                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error adding new user participation");

                return new StatusCodeResult(500);
            }
        }
    }
}
