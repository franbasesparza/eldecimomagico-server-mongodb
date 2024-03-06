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
    public static class PlaGame
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("PlaGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "playgame/{userId}/{gameId}/{bet}")] HttpRequest req,
            ILogger log,
            string userId,
            string gameId,
            int bet)
        {
            log.LogInformation("C# HTTP trigger function processed a request (PlaGame)");

            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<Wallet>.Filter.Eq(w => w.UserId, userId);
                var wallet = db.GetCollection<Wallet>("wallets").Find(filter).FirstOrDefault();

                var filter2 = Builders<Game>.Filter.Eq("_id", ObjectId.Parse(gameId));
                var game = db.GetCollection<Game>("games").Find(filter2).FirstOrDefault();

                if (wallet == null || game == null)
                {
                    return new NotFoundObjectResult(new { error = "Resource not found" });
                }

                if (wallet.Stars < bet)
                {
                    return new StatusCodeResult(406);
                }

                var update = Builders<Wallet>.Update.Set(w => w.Stars, wallet.Stars - bet);
                var result = db.GetCollection<Wallet>("wallets").UpdateOne(filter, update);

                var filter3 = Builders<GameAwardSet>.Filter.Eq("_id", ObjectId.Parse(game.GameAwardSetId));
                var gameAwardSet = db.GetCollection<GameAwardSet>("gameAwardSets").Find(filter3).FirstOrDefault();

                var update2 = Builders<GameAwardSet>.Update
                    .Inc(g => g.PlayedCount, 1);
                var options = new UpdateOptions { IsUpsert = true }; // This option enables upsert behavior
                var result2 = db.GetCollection<GameAwardSet>("gameAwardSets").UpdateOne(filter3, update2, options);

                int randomNumber = new Random().Next(gameAwardSet.MaxRandomNumber);
                int index = 0;
                int i = 0;
                string awardId = "";
                gameAwardSet.GameAwardIds.ForEach(gameAwardId =>
                {
                    if (randomNumber >= i && randomNumber < i + gameAwardSet.GameAwardFavorableOutcomes[index] * bet)
                    {
                        awardId = gameAwardId;
                    }
                    i += gameAwardSet.GameAwardFavorableOutcomes[index] * bet;
                    index++;
                });

                GameResult gameResult = new GameResult
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserId = userId,
                    GameId = gameId,
                    GameAwardId = awardId,
                    StarsBet = bet,
                    Result = randomNumber,
                    CreatedOn = DateTime.UtcNow
                };

                if (awardId != "")
                {
                    db.GetCollection<GameResult>("gameResults").InsertOne(gameResult);
                }

                if (awardId != "")
                {
                    var filter4 = Builders<GameAward>.Filter.Eq("_id", ObjectId.Parse(awardId));
                    var award = db.GetCollection<GameAward>("gameAwards").Find(filter4).FirstOrDefault();

                    var filter5 = Builders<GameAwardType>.Filter.Empty;
                    var gameAwardTypes = db.GetCollection<GameAwardType>("gameAwardTypes").Find(filter5).ToList();
                    var starsAward = gameAwardTypes.FirstOrDefault(g => g.Description == "Stars");
                    var participationAward = gameAwardTypes.FirstOrDefault(g => g.Description == "Participation");
                    var moneyAward = gameAwardTypes.FirstOrDefault(g => g.Description == "Money");

                    //Stars award
                    if (awardId == starsAward.Id)
                    {
                        var filter6 = Builders<Wallet>.Filter.Eq(w => w.UserId, userId);
                        var update3 = Builders<Wallet>.Update
                                    .Set(w => w.Stars, wallet.Stars + award.Quantity);
                        result = db.GetCollection<Wallet>("wallets").UpdateOne(filter, update3);
                    }
                    //Participations Award
                    else if (awardId == participationAward.Id)
                    {
                        var filter6 = Builders<Year>.Filter.Eq(y => y.Description, "Games");
                        var year = db.GetCollection<Year>("years").Find(filter6).FirstOrDefault();

                        Utils.AssignUserAdminParticipation(log, connectionString, dbName, year.Value, award.Quantity, userId, gameResult.Id, "", "");
                        
                    }
                    //Money award
                    else if (awardId == moneyAward.Id)
                    {
                        var filter6 = Builders<Wallet>.Filter.Eq(w => w.UserId, userId);
                        var update3 = Builders<Wallet>.Update
                                    .Set(w => w.Balance, wallet.Balance + award.Quantity);
                        result = db.GetCollection<Wallet>("wallets").UpdateOne(filter, update3);
                    }
                }

                var jsonData = JsonConvert.SerializeObject(gameResult);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(
                        ex,
                        $"Error playing game: {ex.Message}, Stack Trace: {ex.StackTrace}"
                    );


                return new StatusCodeResult(500);
            }
        }
    }
}