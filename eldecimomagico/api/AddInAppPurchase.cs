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
    public static class AddInAppPurchase
    {
        private static string packageName = "io.mcc.edm";
        private static string IOS_PLAYSTORE = "ios-appstore";
        private static string ANDROID_PLAYSTORE = "android-playstore";

        private static string connectionString = Environment.GetEnvironmentVariable(
            "MongoDbConnectionString"
        );
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("AddInAppPurchase")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "post",
                Route = "addinapppurchase/{userId}/{productId}/{storeType}/{purchaseToken}"
            )]
                HttpRequest req,
            ILogger log,
            string userId,
            string productId,
            string storeType,
            string purchaseToken
        )
        {
            log.LogInformation("C# HTTP trigger function processed a request (AddInAppPurchase)");

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    MongoClient client = new MongoClient(connectionString);
                    var db = client.GetDatabase(dbName);

                    if (storeType == ANDROID_PLAYSTORE)
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(
                            $"https://androidpublisher.googleapis.com/androidpublisher/v3/applications/{packageName}/purchases/products/{productId}/tokens/{purchaseToken}"
                        );

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();

                            // Deserialize the JSON string into your custom class
                            GooglePurchaseResponse productPurchase = JsonConvert.DeserializeObject<GooglePurchaseResponse>(responseContent);


                            if (productPurchase.purchaseState == 0)
                            {
                                var inAppPurchase = new InAppPurchase
                                {
                                    Id = ObjectId.GenerateNewId().ToString(),
                                    UserId = userId,
                                    ProductId = productId,
                                    CreatedOn = DateTime.UtcNow
                                };
                                db.GetCollection<InAppPurchase>("inAppPurchases")
                                    .InsertOne(inAppPurchase);

                                int stars = 0;
                                if (InAppPurchase.starMap.TryGetValue(productId, out int value))
                                {
                                    stars = value;
                                }

                                var filter = Builders<Wallet>.Filter.Eq(w => w.UserId, userId);
                                var wallet = db.GetCollection<Wallet>("wallets")
                                    .Find(filter)
                                    .FirstOrDefault();
                                if (wallet != null)
                                {
                                    var update = Builders<Wallet>.Update.Set(
                                        w => w.Stars,
                                        wallet.Stars + stars
                                    );
                                    var result = db.GetCollection<Wallet>("wallets")
                                        .UpdateOne(filter, update);
                                    return new OkResult();
                                }
                                else
                                    return new StatusCodeResult(400);
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
