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
    public static class GetUserAdminParticipations
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("GetUserAdminParticipations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getuseradminparticipations/{year}/{userId}")] HttpRequest req,
            ILogger log,
            int year,
            string userId)
        {
            log.LogInformation("C# HTTP trigger function processed a request (User AdminParticipations)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);
                 
                var filter = Builders<UserAdminParticipation>.Filter.Eq(up => up.UserId, userId) & Builders<UserAdminParticipation>.Filter.Eq(up => up.Year, year);
                var userAdminParticipatons = db.GetCollection<UserAdminParticipation>("userAdminParticipations").Find(filter).ToList();

                var jsonData = JsonConvert.SerializeObject(userAdminParticipatons);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error geting user admin participations");

                return new StatusCodeResult(500);
            }
        }
    }
}
