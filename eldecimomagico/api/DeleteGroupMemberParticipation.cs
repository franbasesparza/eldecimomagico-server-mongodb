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
    public static class DeleteGroupMemberParticipation
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("DeleteGroupMemberParticipation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "deletegroupmemberparticipation/{id}")] HttpRequest req,
            ILogger log,
            string id)
        {

            log.LogInformation($"C# HTTP trigger function processed a request (DeleteGroupMemberParticipation)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<GroupMemberParticipation>.Filter.Eq("_id", new ObjectId(id));
                db.GetCollection<GroupMemberParticipation>("groupMemberParticipations").DeleteOne(filter);

               return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error deleting groupMemberParticipation with ID {id}");
                return new StatusCodeResult(500);
            }
        }
    }
}
