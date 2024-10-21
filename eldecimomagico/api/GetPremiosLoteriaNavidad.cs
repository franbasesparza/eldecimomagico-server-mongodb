using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using eldecimomagico.dal;
using Newtonsoft.Json;

namespace eldecimomagico.api
{
    public static class GetPremiosLoteriaNavidad
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("GetPremiosLoteriaNavidad")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getpremiosloterianavidad/{year}")] HttpRequest req,
            ILogger log,
            int year)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetPremiosLoteriaNavidad).");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<PremioLoteriaNavidad>.Filter.Eq(p => p.Year, year);
                var premiosLoteriaNavidad = db.GetCollection<PremioLoteriaNavidad>("premiosLoteriaNavidad").Find(filter).ToList();

                var jsonData = JsonConvert.SerializeObject(premiosLoteriaNavidad);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(
                        ex,
                        $"Error getting Premios Lotería Navidad games: {ex.Message}, Stack Trace: {ex.StackTrace}"
                    );

                return new StatusCodeResult(500);
            }
        }
    }
}
