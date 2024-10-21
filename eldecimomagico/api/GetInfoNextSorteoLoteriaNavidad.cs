using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MongoDB.Driver;
using eldecimomagico.dal;
using MongoDB.Bson;

namespace eldecimomagico.api
{
    public static class GetInfoNextSorteoLoteriaNavidad
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("GetInfoNextSorteoLoteriaNavidad")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getinfonextsorteoloterianavidad")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetInfoNextSorteoLoteriaNavidad)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<InfoNextSorteoLoteriaNavidad>.Filter.Empty;
                var infoNextSorteoLoteriaNavidad = db.GetCollection<InfoNextSorteoLoteriaNavidad>("infoNextSorteoLoteriaNavidad").Find(filter).FirstOrDefault();

                log.LogInformation($"GetInfoNextSorteoLoteriaNavidad: {JsonConvert.SerializeObject(infoNextSorteoLoteriaNavidad)}");

                if (infoNextSorteoLoteriaNavidad == null)
                {
                    return new NotFoundObjectResult(new { error = "Resource not found" });
                }

                var jsonData = JsonConvert.SerializeObject(infoNextSorteoLoteriaNavidad);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(
                    ex,
                    $"Error getting InfoNextSorteoLoteriaNavidad: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );

                return new StatusCodeResult(500);
            }
        }
    }
}