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
using System.Net.Http;
using System.Text;


namespace eldecimomagico.api
{
    public static class SendNotification
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");


        [FunctionName("SendNotification")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sendNotification")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed (SendNotification).");

            try
            {
                // Read the request body
                string requestBody = await req.ReadAsStringAsync();
                var notification = JsonConvert.DeserializeObject<Notification>(requestBody);

                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                var filter = Builders<NotificationToken>.Filter.Eq(n => n.UserId, notification.UserId);
                var notificationToken = db.GetCollection<NotificationToken>("notificationTokens").Find(filter).FirstOrDefault();

                if (notificationToken != null)
                {
                    var response = await Send(notification, notificationToken.Token, log);
                    
                    var jsonData = JsonConvert.SerializeObject(response);
                    return new OkObjectResult(jsonData);
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(
                    ex,
                    $"Error sending notification: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );

                return new StatusCodeResult(500);
            }
        }

        private static async Task<HttpResponseMessage> Send(Notification notification, string token, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Replace 'YOUR_FCM_SERVER_KEY' with your Firebase Cloud Messaging server key
            string serverKey = "AAAAimAPVWg:APA91bE8inBgl9M9d7aXCFTznB90Rz-yoiS9iXS-jWNJRLIgJS_NIW8IZZjihZcL9H4CxGyxdj9ed4aUbb89fUx9ERq9Tgr4Wb4Zs9Q7fdzKkMgXz_p_aTLllp92v4k3L-AgZ7twU2vi";

            // Replace 'FCM_REGISTRATION_TOKEN' with the actual FCM registration token of the target device
            string registrationToken = token;

            log.LogInformation($"Token: {registrationToken}");
            log.LogInformation($"Title: {notification.Title}");
            log.LogInformation($"Body: {notification.Body}");

            /* HANDLED BY SYSTEM IF APP IS IN BACKGROUND */
            var data = new
            {
                title = notification.Title,
                body = notification.Body
            };

            var message = new
            {
                to = registrationToken,
                notification = new {
                    title = notification.Title,
                    body = notification.Body
                }
            };

            /* HANDLED BY APP
            var data = new
            {
                title = notification.Title,
                body = notification.Body
            };

            var message = new
            {
                to = registrationToken,
                data = data
            };*/

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + serverKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                var json = JsonConvert.SerializeObject(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://fcm.googleapis.com/fcm/send", content);

                return response;
            }
        }
    }
}