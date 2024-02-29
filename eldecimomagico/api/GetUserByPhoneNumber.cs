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
    public static class GetUserByPhoneNumber
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("GetUserByPhoneNumber")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getuserbyphonenumber/{phoneNumber}")] HttpRequest req,
            ILogger log,
            string phoneNumber)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetUser)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<User>.Filter.Eq(u => u.PhoneNumber, phoneNumber);
                var user = db.GetCollection<User>("users").Find(filter).FirstOrDefault();

                if (user == null)
                {
                    return new NotFoundObjectResult(new { error = "User not found" });
                }

                var jsonData = JsonConvert.SerializeObject(user);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(
                        ex,
                        $"Error getting user: {ex.Message}, Stack Trace: {ex.StackTrace}"
                    );

                    return new StatusCodeResult(500);
            }
        }
    }
}