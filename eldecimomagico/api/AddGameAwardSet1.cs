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
using System.Linq;

namespace eldecimomagico.api
{
    public static class AddGameAwardSet1
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("AddGameAwardSet1")]
        public static async Task<IActionResult> Run(
                    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "addgameawardset1")] HttpRequest req,
                    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (AddGameAwardSet1)");

            try
            {

                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<GameAwardType>.Filter.Empty;
                var gameAwardTypes = db.GetCollection<GameAwardType>("gameAwardTypes").Find(filter).ToList();

                var filter2 = Builders<GameAward>.Filter.Empty;
                var gameAwards = db.GetCollection<GameAward>("gameAwards").Find(filter2).ToList();

                GameAwardSet gameAwardSet = new GameAwardSet
                {
                    MaxRandomNumber = 100000,
                    PlayedCount = 0,
                    GameAwardIds = new List<string>(),
                    GameAwardFavorableOutcomes = new List<int>()
                };

                var starsId = gameAwardTypes.FirstOrDefault(g => g.Description == "Stars").Id;
                var gameAward = gameAwards.FirstOrDefault(ga => ga.GameAwardTypeId == starsId && ga.Quantity == 10);
                gameAwardSet.GameAwardIds.Add(gameAward.Id);
                gameAwardSet.GameAwardFavorableOutcomes.Add(20);

                gameAward = gameAwards.FirstOrDefault(ga => ga.GameAwardTypeId == starsId && ga.Quantity == 50);
                gameAwardSet.GameAwardIds.Add(gameAward.Id);
                gameAwardSet.GameAwardFavorableOutcomes.Add(4);

                gameAward = gameAwards.FirstOrDefault(ga => ga.GameAwardTypeId == starsId && ga.Quantity == 100);
                gameAwardSet.GameAwardIds.Add(gameAward.Id);
                gameAwardSet.GameAwardFavorableOutcomes.Add(2);

                gameAward = gameAwards.FirstOrDefault(ga => ga.GameAwardTypeId == starsId && ga.Quantity == 200);
                gameAwardSet.GameAwardIds.Add(gameAward.Id);
                gameAwardSet.GameAwardFavorableOutcomes.Add(1);


                var participationAwardId = gameAwardTypes.FirstOrDefault(g => g.Description == "Participation").Id;
                gameAward = gameAwards.FirstOrDefault(ga => ga.GameAwardTypeId == participationAwardId && ga.Quantity == 1);
                gameAwardSet.GameAwardIds.Add(gameAward.Id);
                gameAwardSet.GameAwardFavorableOutcomes.Add(20);

                gameAward = gameAwards.FirstOrDefault(ga => ga.GameAwardTypeId == participationAwardId && ga.Quantity == 5);
                gameAwardSet.GameAwardIds.Add(gameAward.Id);
                gameAwardSet.GameAwardFavorableOutcomes.Add(4);

                gameAward = gameAwards.FirstOrDefault(ga => ga.GameAwardTypeId == participationAwardId && ga.Quantity == 10);
                gameAwardSet.GameAwardIds.Add(gameAward.Id);
                gameAwardSet.GameAwardFavorableOutcomes.Add(2);

                gameAward = gameAwards.FirstOrDefault(ga => ga.GameAwardTypeId == participationAwardId && ga.Quantity == 20);
                gameAwardSet.GameAwardIds.Add(gameAward.Id);
                gameAwardSet.GameAwardFavorableOutcomes.Add(1);

                db.GetCollection<GameAwardSet>("gameAwardSets").InsertOne(gameAwardSet);

                return new OkResult();
            }
            catch (Exception ex)
            {
                {
                    log.LogError(
                        ex,
                        $"Error adding AddGameAwardSet1: {ex.Message}, Stack Trace: {ex.StackTrace}"
                    );

                    return new StatusCodeResult(500);
                }
            }
        }
    }

}
