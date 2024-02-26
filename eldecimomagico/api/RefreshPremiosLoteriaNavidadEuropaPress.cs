using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.IO;
using eldecimomagico.dal;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace eldecimomagico.api
{
    public static class RefreshPremiosLoteriaNavidadEuropaPress
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("RefreshPremiosLoteriaNavidadEuropaPress")]
        public static async Task RunAsync(
            [TimerTrigger("0 */1 9-15 22 12 *")] TimerInfo myTimer, //{second} {minute} {hour} {day} {month} {day-of-week} "0 */1 9-15 22 12 *"
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request (RefreshPremiosLoteriaNavidadEuropaPress) executed at {DateTime.Now}");

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    MongoClient mongoClient = new MongoClient(connectionString);
                    var db = mongoClient.GetDatabase(dbName);

                    HttpResponseMessage responsePrimeros = await httpClient.GetAsync("https://navidad.europapress.es/rtve/loteria-navidad/primeros.js");
                    HttpResponseMessage responsePedrea = await httpClient.GetAsync("https://navidad.europapress.es/rtve/loteria-navidad/pedrea.js");

                    if (responsePrimeros.IsSuccessStatusCode && responsePedrea.IsSuccessStatusCode)
                    {
                        string primeros = await responsePrimeros.Content.ReadAsStringAsync();
                        string pedrea = await responsePedrea.Content.ReadAsStringAsync();

                        log.LogInformation($"{primeros}");
                        log.LogInformation($"{pedrea}");
                    }
                    else
                    {
                        Console.WriteLine($"Error: {responsePrimeros.StatusCode} - {responsePrimeros.ReasonPhrase} / {responsePedrea.StatusCode} - {responsePedrea.ReasonPhrase}");
                    }

                    DateTime endTime = DateTime.Now;
                    TimeSpan duration = endTime - startTime;
                    log.LogInformation($"Function execution duration: {duration.TotalSeconds} seconds");
                }
                catch (Exception ex)
                {
                    log.LogError(
                    ex,
                    $"Error adding group: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );
                }
            }
        }
    }
}
