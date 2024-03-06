using Microsoft.Extensions.Logging;
using System;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using MongoDB.Driver;
using MongoDB.Bson;
using eldecimomagico.dal;
using Newtonsoft.Json;

namespace eldecimomagico.api
{
    public static class AddInAppSubscription
    {
        private static string packageName = "io.mcc.edm";
        private static string IOS_PLAYSTORE = "ios-appstore";
        private static string ANDROID_PLAYSTORE = "android-playstore";

        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("AddInAppSubscription")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "addinappsubscription/{userId}/{subscriptionTypeId}/{storeType}/{subscriptionToken}")] HttpRequest req,
            ILogger log,
            string userId,
            string subscriptionTypeId,
            string storeType,
            string subscriptionToken
        )
        {
            log.LogInformation("C# HTTP trigger function processed a request (AddInAppSubscription)");

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    MongoClient client = new MongoClient(connectionString);
                    var db = client.GetDatabase(dbName);

                    if (storeType == ANDROID_PLAYSTORE)
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(
                            $"https://androidpublisher.googleapis.com/androidpublisher/v3/applications/{packageName}/purchases/subscriptionsv2/tokens/{subscriptionToken}"
                        );

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();

                            // Deserialize the JSON string into your custom class
                            GoogleSubscriptionPurchaseV2 productSubscription = JsonConvert.DeserializeObject<GoogleSubscriptionPurchaseV2>(responseContent);

                            if (productSubscription.subscriptionState == GoogleInAppSubscriptionState.SUBSCRIPTION_STATE_ACTIVE)
                            {
                                var inAppSubscription = new InAppSubscription
                                {
                                    Id = ObjectId.GenerateNewId().ToString(),
                                    UserId = userId,
                                    SubscriptionTypeId = subscriptionTypeId,
                                    SubscriptionToken = subscriptionToken,
                                    SubscriptionState = GoogleInAppSubscriptionState.SUBSCRIPTION_STATE_ACTIVE,
                                    CreatedOn = DateTime.UtcNow
                                };
                                db.GetCollection<InAppSubscription>("inAppSubscriptions")
                                    .InsertOne(inAppSubscription);

                                int starsAmount = 0;
                                if (InAppPurchase.starMap.TryGetValue(subscriptionTypeId, out int _starsAmount))
                                {
                                    starsAmount = _starsAmount;
                                }

                                var filter = Builders<Wallet>.Filter.Eq(w => w.UserId, userId);
                                var wallet = db.GetCollection<Wallet>("wallets")
                                    .Find(filter)
                                    .FirstOrDefault();
                                if (wallet != null)
                                {
                                    var update = Builders<Wallet>.Update.Set(
                                        w => w.Stars,
                                        wallet.Stars + starsAmount
                                    );
                                    var result = db.GetCollection<Wallet>("wallets")
                                        .UpdateOne(filter, update);
                                    return new OkResult();
                                }

                                var filter2 = Builders<Year>.Filter.Eq(y => y.Description, "Sorteo de Navidad");
                                var year = db.GetCollection<Year>("years").Find(filter2).FirstOrDefault();

                                int participationAmount = 0;
                                if (InAppPurchase.starMap.TryGetValue(subscriptionTypeId, out int _participationAmount))
                                {
                                    participationAmount = _participationAmount;
                                }

                                Utils.AssignUserAdminParticipation(log, connectionString, dbName, year.Value, participationAmount, userId, "", inAppSubscription.Id, "");

                            }
                            else
                                return new StatusCodeResult(400);
                        }
                    }

                    return new StatusCodeResult(400);
                }
                catch (Exception ex)
                {
                    log.LogError(
                        ex,
                        $"Error adding InAppPurchase: {ex.Message}, Stack Trace: {ex.StackTrace}"
                    );

                    return new StatusCodeResult(500);
                }
            }
        }
    }
}
