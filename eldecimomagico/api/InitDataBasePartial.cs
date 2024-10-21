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
using System.Collections.Generic;

namespace eldecimomagico.api
{
    public static class InitDataBasePartial
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("InitDataBasePartial")]
        public static async Task<IActionResult> Run(
                    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "initdatabasepartial")] HttpRequest req,
                    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (InitDataBase)");

            try
            {
                SeedInitDataBase();
                return new OkResult();
            }
            catch (Exception ex)
            {
                {
                    log.LogError(
                        ex,
                        $"Error adding group: {ex.Message}, Stack Trace: {ex.StackTrace}"
                    );

                    return new StatusCodeResult(500);
                }
            }
        }

        private static void SeedInitDataBase()
        {
            MongoClient client = new MongoClient(connectionString);
            var db = client.GetDatabase(dbName);

            var nextDrawDate = DateTime.Parse("2024-12-22 09:00:00");
            var infoNextSorteoLoteriaNavidad = new InfoNextSorteoLoteriaNavidad { DateNextSorteo = nextDrawDate, Status = 0};
            db.GetCollection<InfoNextSorteoLoteriaNavidad>("infoNextSorteoLoteriaNavidad").InsertOne(infoNextSorteoLoteriaNavidad);
        }
    }

}
