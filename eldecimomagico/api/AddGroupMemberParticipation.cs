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
    public static class AddGroupMemberParticipation
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("AddGroupMemberParticipation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "addgroupmemberparticipation")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (AddGroupMemberParticipation)");

            try
            {
                // Read the request body
                string requestBody = await req.ReadAsStringAsync();
                var groupMemberParticipation = JsonConvert.DeserializeObject<GroupMemberParticipation>(requestBody);

                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                groupMemberParticipation.Id = ObjectId.GenerateNewId().ToString();
                groupMemberParticipation.CreatedOn = DateTime.UtcNow;
                db.GetCollection<GroupMemberParticipation>("groupMemberParticipations").InsertOne(groupMemberParticipation);

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error adding new group participation");

                return new StatusCodeResult(500);
            }
        }
    }
}