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
    public static class DeleteUserParticipation
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("DeleteUserParticipation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "deleteuserparticipation/{id}")] HttpRequest req,
            ILogger log,
            string id)
        {

            log.LogInformation($"C# HTTP trigger function processed a request (DeleteUserParticipation)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<UserParticipation>.Filter.Eq("_id", new ObjectId(id));
                db.GetCollection<UserParticipation>("userParticipations").DeleteOne(filter);

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error deleting userParticipation with ID {id}");
                return new StatusCodeResult(500);
            }
        }
    }
}
