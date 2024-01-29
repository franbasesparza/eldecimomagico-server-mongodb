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
using System.Linq;

namespace eldecimomagico.api
{
    public static class GetGroupMembersParticipations
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("GetGroupMembersParticipations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getgroupmembersparticipations/{groupId}")] HttpRequest req,
            ILogger log,
            string groupId)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetGroupMembersParticipations)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<GroupMemberParticipation>.Filter.Eq(gmp => gmp.GroupId, groupId);
                var groupMembersParticipations = db.GetCollection<GroupMemberParticipation>("groupMemberParticipations").Find(filter).ToList();

                var jsonData = JsonConvert.SerializeObject(groupMembersParticipations);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error geting groupMembers");

                return new StatusCodeResult(500);
            }
        }
    }
}