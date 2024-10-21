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
    public static class InitDataBase
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("InitDataBase")]
        public static async Task<IActionResult> Run(
                    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "initdatabase")] HttpRequest req,
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

            var user = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Francisco",
                PhoneNumber = "+34679012369",
                CreatedOn = DateTime.UtcNow,
                IsRegistered = true
            };
            db.GetCollection<User>("users").InsertOne(user);

            var wallet = new Wallet
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = user.Id,
                Balance = 0,
                Stars = 0,
                CreatedOn = DateTime.UtcNow
            };
            db.GetCollection<Wallet>("wallets").InsertOne(wallet);

            var groupTypes = new List<GroupType>
            {
                new GroupType
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "Básico",
                    Description = "Cada miembro del grupo debe aportar un décimo de lotería. Los miembros que obtengan algún premio se comprometen a compartirlo de forma equitativa con el resto."
                },
                new GroupType
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "Libre",
                    Description = "Cada miembro del grupo puede aportar tantos décimos o participaciones como desee. Todos los miembros se comprometen a saldar las cuentas con aquellos que han compartido los décimos o participaciones y los que obtengan algún premio deben compartirlo de forma equitativa con el resto."
                }
            };
            db.GetCollection<GroupType>("groupTypes").InsertMany(groupTypes);

            var gameAwardType1 = new GameAwardType { Id = ObjectId.GenerateNewId().ToString(), Description = "Stars" };
            var gameAwardType2 = new GameAwardType { Id = ObjectId.GenerateNewId().ToString(), Description = "Participation" };
            var gameAwardType3 = new GameAwardType { Id = ObjectId.GenerateNewId().ToString(), Description = "Money" };
            var gameAwardTypes = new List<GameAwardType> { gameAwardType1, gameAwardType2, gameAwardType3 };
            db.GetCollection<GameAwardType>("gameAwardTypes").InsertMany(gameAwardTypes);

            var gameAward1 = new GameAward { Id = ObjectId.GenerateNewId().ToString(), GameAwardTypeId = gameAwardType1.Id, Quantity = 10 };
            var gameAward2 = new GameAward { Id = ObjectId.GenerateNewId().ToString(), GameAwardTypeId = gameAwardType1.Id, Quantity = 50 };
            var gameAward3 = new GameAward { Id = ObjectId.GenerateNewId().ToString(), GameAwardTypeId = gameAwardType1.Id, Quantity = 100 };
            var gameAward4 = new GameAward { Id = ObjectId.GenerateNewId().ToString(), GameAwardTypeId = gameAwardType1.Id, Quantity = 200 };
            var gameAward5 = new GameAward { Id = ObjectId.GenerateNewId().ToString(), GameAwardTypeId = gameAwardType2.Id, Quantity = 1 };
            var gameAward6 = new GameAward { Id = ObjectId.GenerateNewId().ToString(), GameAwardTypeId = gameAwardType2.Id, Quantity = 5 };
            var gameAward7 = new GameAward { Id = ObjectId.GenerateNewId().ToString(), GameAwardTypeId = gameAwardType2.Id, Quantity = 10 };
            var gameAward8 = new GameAward { Id = ObjectId.GenerateNewId().ToString(), GameAwardTypeId = gameAwardType2.Id, Quantity = 20 };
            var gameAwards = new List<GameAward> { gameAward1, gameAward2, gameAward3, gameAward4, gameAward5, gameAward6, gameAward7, gameAward8 };
            db.GetCollection<GameAward>("gameAwards").InsertMany(gameAwards);

            var year = new Year { Id = ObjectId.GenerateNewId().ToString(), Value = 2024, Description = "Games" };
            db.GetCollection<Year>("years").InsertOne(year);
            year = new Year { Id = ObjectId.GenerateNewId().ToString(), Value = 2024, Description = "Sorteo de Navidad" };
            db.GetCollection<Year>("years").InsertOne(year);

            var nextDrawDate = DateTime.Parse("2024-12-22 09:00:00");
            var infoNextSorteoLoteriaNavidad = new InfoNextSorteoLoteriaNavidad { DateNextSorteo = nextDrawDate, Status = 0};
            db.GetCollection<InfoNextSorteoLoteriaNavidad>("infoNextSorteoLoteriaNavidad").InsertOne(infoNextSorteoLoteriaNavidad);

            var adminParticipation = new AdminParticipation
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Number = 00000,
                Year = 0,
                Amount = int.MaxValue,
                Description = "Unasigned admin participation",
                CreatedOn = DateTime.UtcNow,
            };
            db.GetCollection<AdminParticipation>("adminParticipations").InsertOne(adminParticipation);

            var game1 = new Game {Id = ObjectId.GenerateNewId().ToString(), Available = true, Order = 0, Name = "Pachinko", GameAwardSetId = ""};
            var game2 = new Game {Id = ObjectId.GenerateNewId().ToString(), Available = true, Order = 1, Name = "La Ruleta de La Suerte", GameAwardSetId = ""};
            var game3 = new Game {Id = ObjectId.GenerateNewId().ToString(), Available = true, Order = 2, Name = "Rasca Pica", GameAwardSetId = ""};
            var game4 = new Game {Id = ObjectId.GenerateNewId().ToString(), Available = true, Order = 3, Name = "El Penalty Decisivo", GameAwardSetId = ""};
            var game5 = new Game {Id = ObjectId.GenerateNewId().ToString(), Available = true, Order = 4, Name = "El Coche Fantástico", GameAwardSetId = ""};
            var game6 = new Game {Id = ObjectId.GenerateNewId().ToString(), Available = true, Order = 5, Name = "El Forzudo del Circo", GameAwardSetId = ""};
            var game7 = new Game {Id = ObjectId.GenerateNewId().ToString(), Available = true, Order = 6, Name = "Ron Boy Run", GameAwardSetId = ""};
            var game8 = new Game {Id = ObjectId.GenerateNewId().ToString(), Available = true, Order = 7, Name = "Ale Hop", GameAwardSetId = ""};
            var games = new List<Game> { game1, game2, game3, game4, game5, game6, game7, game8 };
            db.GetCollection<Game>("games").InsertMany(games);

        }
    }

}
