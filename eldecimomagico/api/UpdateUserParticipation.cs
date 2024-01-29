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
    public class UpdateUserParticipation
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("UpdateUserParticipation")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "updateuserparticipation")] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (UpdateParticipation).");

            try
            {
                // Read the request body
                string requestBody = await req.ReadAsStringAsync();
                var userParticipation = JsonConvert.DeserializeObject<UserParticipation>(requestBody);

                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<UserParticipation>.Filter.Eq(up => up.Id, userParticipation.Id);
                var result = db.GetCollection<UserParticipation>("userParticipations").ReplaceOne(filter, userParticipation);

                if (!(result.IsAcknowledged && result.ModifiedCount > 0))
                {
                    return new NotFoundObjectResult(new { error = "Error updating user particiaption" });
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error updating wallet");

                return new StatusCodeResult(500);
            }
        }
    }
}
