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
using System.Collections.Generic;

namespace eldecimomagico.api
{
    public static class GetGroups
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("GetGroups")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getgroups/{userId}")] HttpRequest req,
            ILogger log,
            string userId)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetGroups)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<GroupMember>.Filter.Eq(gm => gm.UserId, userId);
                var groupMembers = db.GetCollection<GroupMember>("groupMembers").Find(filter).ToList();

                // Extract GroupIds from the found GroupMembers
                var groupIds = groupMembers.Select(gm => gm.GroupId).ToList();
                List<ObjectId> objectIdList = groupIds.Select(ObjectId.Parse).ToList();
                
                // Query the Groups collection for details
                var groupsFilter = Builders<Group>.Filter.In("_id", objectIdList);
                var groups = db.GetCollection<Group>("groups").Find(groupsFilter).ToList();
                
                var jsonData = JsonConvert.SerializeObject(groups);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error geting groups");

                return new StatusCodeResult(500);
            }
        }
    }
}