using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;

namespace eldecimomagico.api
{
    public static class RefreshPremiosLoteriaNavidad
    {
        //https://www.rtve.es/loterias/loteria-navidad/Loteria_00000.shtml
        //https://pabloclementeperez.com/2023/12/31/pesadilla-antes-de-navidad-con-la-app-de-la-loteria/


        [FunctionName("RefreshPremiosLoteriaNavidad")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "refreshpremiosloterianavidad")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (RefreshPremiosLoteriaNavidad).");
            //string[] suffixArray = new string[] {"00000","05000","10000","15000","20000","25000","30000","35000","40000","45000","50000","55000","60000","65000","70000","75000","80000","85000","90000","95000"};
            string[] suffixArray = new string[] {"00000"};
            foreach (string suffix in suffixArray){
                GetPremiosLoteriaNavidad(suffix,log);
            }

            return new OkResult();
        }

        private static void GetPremiosLoteriaNavidad(string suffix,ILogger log)
        {
            string url = $"https://www.rtve.es/loterias/loteria-navidad/Loteria_{suffix}.shtml";

            log.LogInformation($"url: {url}");

            // Load the HTML document from the URL
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            log.LogInformation($"doc: {doc.Text}");

            // XPath expression to select the table rows containing the numbers and prices
            string xpathExpression = "//div[@class='tabla-loteria']/table[@id='millares']/tbody/tr";

            // Select the table rows using the XPath expression
            HtmlNodeCollection rows = doc.DocumentNode.SelectNodes(xpathExpression);
            if (rows != null)
            {
                // Iterate over each table row
                foreach (HtmlNode row in rows)
                {
                    // Extract the number and price from each row
                    string number = row.ChildNodes[1].InnerText.Trim();
                    string price = row.ChildNodes[3].InnerText.Trim();

                    // Output the number and price
                    log.LogInformation("{number}:  {price}€");
                }
            }
            else
            {
                log.LogInformation("No data found.");
            }

        }
    }
}
