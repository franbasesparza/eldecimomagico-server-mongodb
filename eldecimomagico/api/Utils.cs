using System;
using eldecimomagico.dal;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace eldecimomagico.api
{
    public static class Utils
    {
        public static bool AssignUserAdminParticipation(ILogger log, string connectionString, string dbName, int year, int amount, string userId, string gameResultId, string inAppSubscriptionId, string purchaseId)
        {
            try
            {
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<Year>.Filter.Eq(y => y.Description, "Sorteo de Navidad");
                
                var filter2 = Builders<AdminParticipation>.Filter.Eq(a => a.Year, year);
                var adminParticipations = db.GetCollection<AdminParticipation>("adminParticipations").Find(filter2).ToList();
                var notFound = true;
                var i = 0;
                while (notFound && i < adminParticipations.Count)
                {
                    string targetAdminParticipationId = adminParticipations[i].Id;
                    var filter3 = Builders<UserAdminParticipation>.Filter.Eq(u => u.AdminParticipationId, targetAdminParticipationId);
                    var sumResult = db.GetCollection<UserAdminParticipation>("userAdminParticipations")
                        .Aggregate()
                        .Match(filter3)
                        .Group(new BsonDocument { { "_id", BsonNull.Value }, { "totalAmount", new BsonDocument("$sum", "$amount") } })
                        .FirstOrDefault();
                    float totalAmount = sumResult?.GetValue("totalAmount", 0).ToInt32() ?? 0.0f;

                    if (totalAmount + amount <= adminParticipations[i].Amount)
                    {
                        var userAdminParticipation = new UserAdminParticipation
                        {
                            Id = ObjectId.GenerateNewId().ToString(),
                            UserId = userId,
                            AdminParticipationId = adminParticipations[i].Id,
                            Year = year,
                            Amount = amount,
                            GameResultId = gameResultId,
                            InAppSubscriptionId = inAppSubscriptionId,
                            PurchaseId = purchaseId,
                            CreatedOn = DateTime.UtcNow
                        };
                        db.GetCollection<UserAdminParticipation>("userAdminParticipations").InsertOne(userAdminParticipation);
                        notFound = false;
                    }
                }
                if (notFound)
                {
                    var filter4 = Builders<AdminParticipation>.Filter.Eq(a => a.Year, 0);
                    var adminParticipation = db.GetCollection<AdminParticipation>("adminParticipations").Find(filter4).FirstOrDefault();
                    var userAdminParticipation = new UserAdminParticipation
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = userId,
                        AdminParticipationId = adminParticipation.Id,
                        Year = year,
                        Amount = amount,
                        GameResultId = gameResultId,
                        InAppSubscriptionId = inAppSubscriptionId,
                        PurchaseId = purchaseId,
                        CreatedOn = DateTime.UtcNow
                    };
                    db.GetCollection<UserAdminParticipation>("userAdminParticipations").InsertOne(userAdminParticipation);
                }
                return true;
            }
            catch (Exception ex)
            {
                log.LogError(
                        ex,
                        $"Error playing game: {ex.Message}, Stack Trace: {ex.StackTrace}"
                    );

                return false;
            }
        }
    }
}