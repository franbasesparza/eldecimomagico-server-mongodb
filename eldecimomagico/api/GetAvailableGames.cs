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
    public static class GetAvailableGames
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("GetAvailableGames")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getavailablegames")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetAvailableGames)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<Game>.Filter.Eq(g => g.Available, true);
                var games = db.GetCollection<Game>("games").Find(filter).ToList();

                var jsonData = JsonConvert.SerializeObject(games);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error geting group types");

                return new StatusCodeResult(500);
            }
        }
    }
}