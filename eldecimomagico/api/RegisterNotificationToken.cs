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
using Microsoft.Extensions.Primitives;
using System.Net;

namespace eldecimomagico.api
{
    public static class AddNotificationToken
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("AddNotificationToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "addnotificationtoken")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (AddNotificationToken)");

            try
            {
                // Read the request body
                string requestBody = await req.ReadAsStringAsync();
                var newNotificationToken = JsonConvert.DeserializeObject<NotificationToken>(requestBody);

                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<NotificationToken>.Filter.Eq(n => n.UserId, newNotificationToken.UserId);
                var notificationToken = db.GetCollection<NotificationToken>("notificationTokens").Find(filter).FirstOrDefault();

                if (notificationToken == null)
                {
                    notificationToken = new NotificationToken
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = newNotificationToken.UserId,
                        Token = newNotificationToken.Token
                    };
                    db.GetCollection<NotificationToken>("notificationTokens").InsertOne(notificationToken);
                }
                else
                {
                    var update = Builders<NotificationToken>.Update
                            .Set(n => n.Token, newNotificationToken.Token);

                    var result = db.GetCollection<NotificationToken>("notificationTokens").UpdateOne(filter, update);
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(
                    ex,
                    $"Error adding notification token: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );

                return new StatusCodeResult(500);
            }
        }
    }
}