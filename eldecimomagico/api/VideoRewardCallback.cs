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
using Microsoft.Extensions.Primitives;
using System.Net;

namespace eldecimomagico.api
{
    public static class VideoRewardCallback
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("VideoRewardCallback")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "videorewardcallback")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (VideoRewardCallback)");

            try
            {
                // Get query parameters from the URL
                string adNetwork = req.Query["ad_network"];
                string adUnit = req.Query["ad_unit"];
                int rewardAmount = Convert.ToInt32(req.Query["reward_amount"]);
                string rewardItem = req.Query["reward_item"];
                long timestamp = Convert.ToInt64(req.Query["timestamp"]);
                string transactionId = req.Query["transaction_id"];
                string userId = req.Query["user_id"];
                string keyId = req.Query["key_id"];

                // Log the extracted query parameters
                log.LogInformation($"ad_network: {adNetwork}");
                log.LogInformation($"ad_unit: {adUnit}");
                log.LogInformation($"reward_amount: {rewardAmount}");
                log.LogInformation($"reward_item: {rewardItem}");
                log.LogInformation($"timestamp: {timestamp}");
                log.LogInformation($"transaction_id: {transactionId}");
                log.LogInformation($"user_id: {userId}");
                log.LogInformation($"key_id: {keyId}");

                // Perform ad verification logic here
                bool isHostGoogle = true;
                bool isAdVerified = true;

                if (isHostGoogle && isAdVerified)
                {
                    MongoClient client = new MongoClient(connectionString);
                    var db = client.GetDatabase(dbName);

                    var filter = Builders<Wallet>.Filter.Eq(w => w.UserId, userId);
                    var wallet = db.GetCollection<Wallet>("wallets").Find(filter).FirstOrDefault();

                    if (wallet != null)
                    {
                        var update = Builders<Wallet>.Update
                                                    .Set(w => w.Stars, wallet.Stars + rewardAmount);
                        var result = db.GetCollection<Wallet>("wallets").UpdateOne(filter, update);
                    }

                    return new OkResult();
                }
                else
                {
                    return new StatusCodeResult(500);
                }
            }
            catch (Exception ex)
            {
                log.LogError(
                    ex,
                    $"Error verifying view reward: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );

                return new StatusCodeResult(500);
            }
        }
    }
}