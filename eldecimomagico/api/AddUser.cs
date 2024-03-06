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
    public static class AddUser
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("AddUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "adduser")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (AddUser)");

            try
            {
                string requestBody = await req.ReadAsStringAsync();
                log.LogInformation("requestBody: " + requestBody);
                User user = JsonConvert.DeserializeObject<User>(requestBody);

                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var existingUserFilter = Builders<User>.Filter.Eq(u => u.Email, user.Email);
                var existingUser = db.GetCollection<User>("users").Find(existingUserFilter).FirstOrDefault();

                if (existingUser != null)
                {
                    // User exists, update phone number
                    var updateDefinition = Builders<User>.Update.Set(u => u.PhoneNumber, user.PhoneNumber);
                    db.GetCollection<User>("users").UpdateOne(existingUserFilter, updateDefinition);
                }
                else
                {
                    // User does not exist, insert new user
                    user.Id = ObjectId.GenerateNewId().ToString();
                    user.IsRegistered = true;
                    user.CreatedOn = DateTime.UtcNow;
                    db.GetCollection<User>("users").InsertOne(user);

                    //Create wallet
                    var wallet = new Wallet
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = user.Id
                    };
                    db.GetCollection<Wallet>("wallets").InsertOne(wallet);

                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error adding user");

                return new StatusCodeResult(500);
            }
        }
    }
}