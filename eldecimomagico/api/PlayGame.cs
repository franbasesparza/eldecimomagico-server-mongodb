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

                var filter3 = Builders<GameAwardSet>.Filter.Eq("_id", ObjectId.Parse(game.GameAwardSetId));
                var gameAwardSet = db.GetCollection<GameAwardSet>("games").Find(filter3).FirstOrDefault();


                int randomNumber = new Random().Next(gameAwardSet.MaxRandomNumber);
                int i = 0;
                string awardId = "";
                gameAwardSet.GameAwardIds.ForEach(gameAwardId =>
                {
                    if (randomNumber >= i && randomNumber < i + gameAwardSet.GameAwardFavorableOutcomes[i])
                    {
                        awardId = gameAwardId;
                    }
                    i += gameAwardSet.GameAwardFavorableOutcomes[i];
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
                db.GetCollection<GameResult>("gameResults").InsertOne(gameResult);

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
                        var update = Builders<Wallet>.Update
                                    .Set(w => w.Stars, wallet.Stars + award.Quantity);
                        var result = db.GetCollection<Wallet>("wallets").UpdateOne(filter, update);
                    }
                    //Participations Award
                    else if (awardId == participationAward.Id)
                    {
                        var filter6 = Builders<Year>.Filter.Eq(y => y.Description, "Games");
                        var year = db.GetCollection<Year>("years").Find(filter6).FirstOrDefault();

                        var filter7 = Builders<AdminParticipation>.Filter.Eq(a => a.Year, year.Value);
                        var adminParticipations = db.GetCollection<AdminParticipation>("adminParticipations").Find(filter7).ToList();
                        var notFound = true;
                        i = 0;
                        while (notFound && i < adminParticipations.Count)
                        {
                            string targetAdminParticipationId = adminParticipations[i].Id;
                            var filter8 = Builders<UserAdminParticipation>.Filter.Eq(u => u.AdminParticipationId, targetAdminParticipationId);
                            var sumResult = db.GetCollection<UserAdminParticipation>("userAdminParticipations")
                                .Aggregate()
                                .Match(filter8)
                                .Group(new BsonDocument { { "_id", BsonNull.Value }, { "totalAmount", new BsonDocument("$sum", "$amount") } })
                                .FirstOrDefault();
                            float totalAmount = sumResult?.GetValue("totalAmount", 0).ToInt32() ?? 0.0f;

                            if (totalAmount + award.Quantity <= adminParticipations[i].Amount)
                            {
                                var userAdminParticipation = new UserAdminParticipation
                                {
                                    Id = ObjectId.GenerateNewId().ToString(),
                                    UserId = userId,
                                    AdminParticipationId = adminParticipations[i].Id,
                                    Amount = award.Quantity,
                                    GameResultId = gameResult.Id,
                                    PurchaseId = "",
                                    CreatedOn = DateTime.UtcNow
                                };
                                db.GetCollection<UserAdminParticipation>("userAdminParticipations").InsertOne(userAdminParticipation);
                                notFound = false;
                            }
                        }
                        if (notFound)
                        {
                            var filter8 = Builders<AdminParticipation>.Filter.Eq(a => a.Year, 0);
                            var adminParticipation = db.GetCollection<AdminParticipation>("adminParticipations").Find(filter8).FirstOrDefault();
                            var userAdminParticipation = new UserAdminParticipation
                                {
                                    Id = ObjectId.GenerateNewId().ToString(),
                                    UserId = userId,
                                    AdminParticipationId = adminParticipation.Id,
                                    Amount = award.Quantity,
                                    GameResultId = gameResult.Id,
                                    PurchaseId = "",
                                    CreatedOn = DateTime.UtcNow
                                };
                                db.GetCollection<UserAdminParticipation>("userAdminParticipations").InsertOne(userAdminParticipation);
                        }
                    }
                    //Money award
                    else if (awardId == moneyAward.Id)
                    {
                        var filter6 = Builders<Wallet>.Filter.Eq(w => w.UserId, userId);
                        var update = Builders<Wallet>.Update
                                    .Set(w => w.Balance, wallet.Balance + award.Quantity);
                        var result = db.GetCollection<Wallet>("wallets").UpdateOne(filter, update);
                    }
                }

                var jsonData = JsonConvert.SerializeObject(gameResult);
                return new OkObjectResult(jsonData);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error geting wallet");

                return new StatusCodeResult(500);
            }
        }
    }
}