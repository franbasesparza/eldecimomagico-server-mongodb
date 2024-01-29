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
    public static class GetGroupTypes
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("GetGroupTypes")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getgrouptypes")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetGroupTypes)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<GroupType>.Filter.Empty;
                var groupTypes = db.GetCollection<GroupType>("groupTypes").Find(filter).ToList();

                var jsonData = JsonConvert.SerializeObject(groupTypes);
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