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
    public static class GetGroupMembers
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("GetGroupMembers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getgroupmembers/{groupId}")] HttpRequest req,
            ILogger log,
            string groupId)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetGroups)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<GroupMember>.Filter.Eq(gm => gm.GroupId, groupId);
                var groupMembers = db.GetCollection<GroupMember>("groupMembers").Find(filter).ToList();

                var jsonData = JsonConvert.SerializeObject(groupMembers);
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