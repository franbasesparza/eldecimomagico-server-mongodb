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
    public static class GetUserParticipations
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("GetUserParticipations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getuserparticipations/{year}/{userId}")] HttpRequest req,
            ILogger log,
            int year,
            string userId)
        {
            log.LogInformation("C# HTTP trigger function processed a request (Participations)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<UserParticipation>.Filter.Eq(up => up.UserId, userId) & Builders<UserParticipation>.Filter.Eq(up => up.Year, year);
                var userParticipatons = db.GetCollection<UserParticipation>("userParticipations").Find(filter).ToList();

                var jsonData = JsonConvert.SerializeObject(userParticipatons);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error geting user participations");

                return new StatusCodeResult(500);
            }
        }
    }
}
