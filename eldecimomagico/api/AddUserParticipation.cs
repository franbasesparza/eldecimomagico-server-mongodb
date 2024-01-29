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
    public static class AddUserParticipation
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("AddUserParticipation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "adduserparticipation")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (AddUserParticipation)");

            try
            {
                // Read the request body
                string requestBody = await req.ReadAsStringAsync();
                var userParticipation = JsonConvert.DeserializeObject<UserParticipation>(requestBody);

                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                //Create user participation
                userParticipation.Id = ObjectId.GenerateNewId().ToString();
                db.GetCollection<UserParticipation>("userParticipations").InsertOne(userParticipation);

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
