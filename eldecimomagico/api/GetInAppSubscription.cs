using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MongoDB.Driver;
using eldecimomagico.dal;
using MongoDB.Bson;

namespace eldecimomagico.api
{
    public static class GetInAppSubscription
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("GetInAppSubscription")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getinappsubscription/{userId}")] HttpRequest req,
            ILogger log,
            string userId)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetInAppSubscription)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<InAppSubscription>.Filter.Eq(s => s.UserId, userId);
                var inAppSubscription = db.GetCollection<InAppSubscription>("inAppSubscriptions").Find(filter).FirstOrDefault();

                if (inAppSubscription == null)
                {
                    return new NotFoundObjectResult(new { error = "Resource not found" });
                }

                var jsonData = JsonConvert.SerializeObject(inAppSubscription);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(
                    ex,
                    $"Error getting inAppSubscription: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );

                return new StatusCodeResult(500);
            }
        }
    }
}