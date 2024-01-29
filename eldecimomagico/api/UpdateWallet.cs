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
    public static class UpdateWallet
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("UpdateWallet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "updatewallet")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to update (Wallet).");

            try
            {
                // Read the request body
                string requestBody = await req.ReadAsStringAsync();
                var wallet = JsonConvert.DeserializeObject<Wallet>(requestBody);
                
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<Wallet>.Filter.Eq(w => w.Id, wallet.Id);
                var update = Builders<Wallet>.Update
                            .Set(w => w.Balance, wallet.Balance)
                            .Set(w => w.Stars, wallet.Stars);
                var result = db.GetCollection<Wallet>("wallets").UpdateOne(filter, update);

                log.LogInformation($"{result.IsAcknowledged} {result.MatchedCount} {result.ModifiedCount} {result.UpsertedId}");

                if (!(result.IsAcknowledged && result.ModifiedCount > 0))
                {
                    return new NotFoundObjectResult(new { error = "Error updating wallet" });
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(
                    ex,
                    $"Error adding wallet: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );

                return new StatusCodeResult(500);
            }
        }
    }
}