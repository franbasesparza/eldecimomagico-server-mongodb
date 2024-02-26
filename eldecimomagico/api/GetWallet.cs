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
    public static class GetWallet
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("GetWallet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getwallet/{userId}")] HttpRequest req,
            ILogger log,
            string userId)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetWallet)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<Wallet>.Filter.Eq(w => w.UserId, userId);
                var wallet = db.GetCollection<Wallet>("wallets").Find(filter).FirstOrDefault();

                if (wallet == null)
                {
                    return new NotFoundObjectResult(new { error = "Resource not found" });
                }

                var jsonData = JsonConvert.SerializeObject(wallet);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(
                    ex,
                    $"Error getting wallet: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );

                return new StatusCodeResult(500);
            }
        }
    }
}