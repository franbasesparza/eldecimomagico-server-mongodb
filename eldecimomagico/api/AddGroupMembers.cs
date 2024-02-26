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
    public static class AddGroupMembers
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("AddGroupMembers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "addgroupmembers")] HttpRequest req,
            ILogger log
        )
        {
            log.LogInformation("C# HTTP trigger function processed a request (AddGroupMembers)");

            try
            {
                string requestBody = await req.ReadAsStringAsync();
                log.LogInformation("requestBody: " + requestBody);
                GroupMember[] groupMembers = JsonConvert.DeserializeObject<GroupMember[]>(requestBody);

                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                foreach (var member in groupMembers)
                {
                    member.Id = ObjectId.GenerateNewId().ToString();
                    member.CreatedOn = DateTime.UtcNow;

                    var filter = Builders<User>.Filter.Eq(u => u.PhoneNumber, member.PhoneNumber);
                    User user = db.GetCollection<User>("users").Find(filter).FirstOrDefault();
                    
                    if (user == null)
                    {
                        // Create non existing user
                        user = new User
                        {
                            Id = ObjectId.GenerateNewId().ToString(),
                            PhoneNumber = member.PhoneNumber,
                            IsRegistered = false,
                            CreatedOn = DateTime.UtcNow
                        };
                        db.GetCollection<User>("users").InsertOne(user);
                    }
                    member.UserId = user.Id;
                }
                
                db.GetCollection<GroupMember>("groupMembers").InsertMany(groupMembers);

                var filter2 = Builders<GroupMember>.Filter.Eq(gm => gm.GroupId, groupMembers[0].GroupId);
                var groupMembers2 = db.GetCollection<GroupMember>("groupMembers").Find(filter2).ToList();

                var jsonData = JsonConvert.SerializeObject(groupMembers2);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(
                    ex,
                    $"Error adding group: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );

                return new StatusCodeResult(500);
            }
        }
    }
}
