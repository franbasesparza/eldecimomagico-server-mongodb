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

namespace eldecimomagico.api
{
    public static class RefreshPremiosLoteriaNavidadRTVE
    {
        //https://www.rtve.es/loterias/loteria-navidad/Loteria_00000.shtml
        //https://pabloclementeperez.com/2023/12/31/pesadilla-antes-de-navidad-con-la-app-de-la-loteria/

        private static string connectionString = Environment.GetEnvironmentVariable("MongoDbConnectionString");
        private static string dbName = Environment.GetEnvironmentVariable("MongoDbName");

        [FunctionName("RefreshPremiosLoteriaNavidadRTVE")]
        public static void Run(
            [TimerTrigger("0 */10 9-15 22 12 *")] TimerInfo myTimer, //{second} {minute} {hour} {day} {month} {day-of-week} "0 */10 9-15 22 12 *"
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request (RefreshPremiosLoteriaNavidadRTVE) executed at {DateTime.Now}");

            try
            {
                DateTime startTime = DateTime.Now;
                MongoClient client = new MongoClient(connectionString);
                var db = client.GetDatabase(dbName);

                string[] suffixArray = new string[] {"00000","05000","10000","15000","20000","25000","30000","35000","40000","45000","50000","55000","60000","65000","70000","75000","80000","85000","90000","95000"};
                //string[] suffixArray = new string[] { "85000" };
                foreach (string suffix in suffixArray)
                {
                    GetPremiosLoteriaNavidad(db, suffix, log);
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

        private static void GetPremiosLoteriaNavidad(IMongoDatabase db, string suffix, ILogger log)
        {
            string url = $"https://www.rtve.es/loterias/loteria-navidad/Loteria_{suffix}.shtml";

            log.LogInformation($"url: {url}");

            // Load the HTML document from the URL
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            // XPath expression to select the table rows containing the numbers and prices
            string xpathExpression = "//table[@id='millares']/tr/td//table//tr";
            HtmlNodeCollection rows = doc.DocumentNode.SelectNodes(xpathExpression);

            int count = 0;
            IMongoCollection<PremioLoteriaNavidad> collection = db.GetCollection<PremioLoteriaNavidad>("premiosLoteriaNavidad");
            if (rows != null)
            {
                // Iterate over each table row
                foreach (HtmlNode row in rows)
                {

                    HtmlNode numberNode = row.SelectSingleNode(".//*[contains(@class, 'n')]");
                    HtmlNode priceNode = row.SelectSingleNode(".//*[contains(@class, 'p')]");

                    if (numberNode != null && priceNode != null)
                    {
                        // Extract the number and price from each row
                        int number = int.Parse(numberNode.InnerText.Trim().Replace(".", ""));
                        int price = int.Parse(priceNode.InnerText.Trim().Replace(".", "").Replace("&euro;", ""));

                        //Create wallet
                        var premioLoteriaNavidad = new PremioLoteriaNavidad
                        {
                            Year = 2023,
                            Number = number,
                            Amount = price
                        };

                        var filter = Builders<PremioLoteriaNavidad>.Filter.Eq(p => p.Year, premioLoteriaNavidad.Year) &
                                     Builders<PremioLoteriaNavidad>.Filter.Eq(p => p.Number, premioLoteriaNavidad.Number);

                        var update = Builders<PremioLoteriaNavidad>.Update
                            .Set(p => p.Amount, premioLoteriaNavidad.Amount)
                            .SetOnInsert(p => p.Id, ObjectId.GenerateNewId().ToString());

                        // Perform upsert operation (insert if not exists, update if exists)
                        collection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

                        count++;
                    }
                }
            }
            else
            {
                log.LogInformation("No data found.");
            }

            log.LogInformation($"Awarded numbers ({suffix}): {count}");
        }
    }
}
