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
using System.Text.RegularExpressions;
using System.Linq;

namespace eldecimomagico.api
{
    public static class RefreshPremiosLoteriaNavidadEuropaPress
    {
        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("RefreshPremiosLoteriaNavidadEuropaPress")]
        public static async Task RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "refreshpremiosloterianavidadeuroparess")] HttpRequest req,
            //[TimerTrigger("0 */1 9-15 22 12 *")] TimerInfo myTimer, //{second} {minute} {hour} {day} {month} {day-of-week} "0 */1 9-15 22 12 *"
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request (RefreshPremiosLoteriaNavidadEuropaPress) executed at {DateTime.Now}!");

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
                        //log.LogInformation($"{pedrea}");

                        ParsePrimeros(log, db, primeros);
                        ParsePedrea(log, db, pedrea);

                    }
                    else
                    {
                        log.LogError($"Error: {responsePrimeros.StatusCode} - {responsePrimeros.ReasonPhrase} / {responsePedrea.StatusCode} - {responsePedrea.ReasonPhrase}");
                    }

                    DateTime endTime = DateTime.Now;
                    TimeSpan duration = endTime - startTime;
                    log.LogInformation($"Function execution duration: {duration.TotalSeconds} seconds");
                }
                catch (Exception ex)
                {
                    log.LogError(
                    ex,
                    $"Error adding premio: {ex.Message}, Stack Trace: {ex.StackTrace}"
                );
                }
            }
        }

        private static void ParsePrimeros(ILogger log, IMongoDatabase db, string primeros)
        {
            IMongoCollection<PremioLoteriaNavidad> collection = db.GetCollection<PremioLoteriaNavidad>("premiosLoteriaNavidad");

            var primerosMatch = Regex.Match(primeros, @"var\s+primeros\s*=\s*\[(.*?)\];", RegexOptions.Singleline);
            //log.LogInformation($"primerosMatch: {primerosMatch.Success} -*- {primerosMatch.Groups[1].Value}");
            var numbersArray = primerosMatch.Groups[1].Value
                .Split(',')
                .Select(s => string.IsNullOrWhiteSpace(s) ? -1 : int.Parse(s.Trim()))
                .ToArray();

            // Regular expression to extract "primerosActualizacion"
            var primerosActualizacionMatch = Regex.Match(primeros, @"var primerosActualizacion = ""(.+?)""");
            var primerosUpdatedOn = DateTime.Parse(primerosActualizacionMatch.Groups[1].Value);

            //log.LogInformation($"{numbersArray} - {primerosUpdatedOn}");

            for (var i = 0; i < numbersArray.Count(); i++)
            {
                //TEST ONLY
                primerosUpdatedOn = GetRandomDateTime();//DateTime.Parse(primerosActualizacionMatch.Groups[1].Value);

                var filter = Builders<PremioLoteriaNavidad>.Filter.Eq(p => p.Year, primerosUpdatedOn.Year) &
                             Builders<PremioLoteriaNavidad>.Filter.Eq(p => p.Number, numbersArray[i]);

                var update = Builders<PremioLoteriaNavidad>.Update
                    .Set(p => p.Year, primerosUpdatedOn.Year)
                    .Set(p => p.Number, numbersArray[i])
                    .Set(p => p.Type, IndexToType(i))
                    .Set(p => p.Amount, (int)IndexToAmount(i))
                    .Set(p => p.UpdatedOn, primerosUpdatedOn)
                    .SetOnInsert(p => p.Id, ObjectId.GenerateNewId().ToString());

                // Perform upsert operation (insert if not exists, update if exists)
                collection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
            }
        }

        private static void ParsePedrea(ILogger log, IMongoDatabase db, string pedrea)
        {
        }

        private static PremioLoteriaNavidadType IndexToType(int index)
        {
            switch (index)
            {
                case 0:
                    return PremioLoteriaNavidadType.PRIMER_PREMIO;
                case 1:
                    return PremioLoteriaNavidadType.SEGUNDO_PREMIO;
                case 2:
                    return PremioLoteriaNavidadType.TERCER_PREMIO;
                case 3:
                    return PremioLoteriaNavidadType.CUARTO_PREMIO_1;
                case 4:
                    return PremioLoteriaNavidadType.CUARTO_PREMIO_2;
                case 5:
                    return PremioLoteriaNavidadType.QUINTO_PREMIO_1;
                case 6:
                    return PremioLoteriaNavidadType.QUINTO_PREMIO_2;
                case 7:
                    return PremioLoteriaNavidadType.QUINTO_PREMIO_3;
                case 8:
                    return PremioLoteriaNavidadType.QUINTO_PREMIO_4;
                case 9:
                    return PremioLoteriaNavidadType.QUINTO_PREMIO_5;
                case 10:
                    return PremioLoteriaNavidadType.QUINTO_PREMIO_6;
                case 11:
                    return PremioLoteriaNavidadType.QUINTO_PREMIO_7;
                case 12:
                    return PremioLoteriaNavidadType.QUINTO_PREMIO_8;
                default:
                    return PremioLoteriaNavidadType.UNKNOWN;
            }
        }

        private static PremioLoteriaNavidadAmount IndexToAmount(int index)
        {
            switch (index)
            {
                case 0:
                    return PremioLoteriaNavidadAmount.PRIMER_PREMIO;
                case 1:
                    return PremioLoteriaNavidadAmount.SEGUNDO_PREMIO;
                case 2:
                    return PremioLoteriaNavidadAmount.TERCER_PREMIO;
                case 3:
                case 4:
                    return PremioLoteriaNavidadAmount.CUARTO_PREMIO;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                    return PremioLoteriaNavidadAmount.QUINTO_PREMIO;
                default:
                    return PremioLoteriaNavidadAmount.UNKNOWN;
            }
        }

        public static DateTime GetRandomDateTime()
        {
            // Define start and end time
            DateTime startTime = new DateTime(2023, 12, 22, 9, 0, 0); // 22 Dec 2023 09:00
            DateTime endTime = new DateTime(2023, 12, 22, 14, 0, 0);  // 22 Dec 2023 14:00

            // Get the total minutes between start and end time
            int totalMinutes = (int)(endTime - startTime).TotalMinutes;

            // Generate a random number of minutes
            Random random = new Random();
            int randomMinutes = random.Next(0, totalMinutes);

            // Add random minutes to the start time
            DateTime randomDateTime = startTime.AddMinutes(randomMinutes);

            return randomDateTime;
        }
    }
}
