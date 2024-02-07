using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace eldecimomagico.api
{
    public static class GetPremiosLoteriaNavidad
    {
        private static readonly string API_EL_PAIS_URL = "https://api.elpais.com/ws/LoteriaNavidadPremiados";
        
        [FunctionName("GetPremiosLoteriaNavidad")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getpremiosloterianavidad")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request (GetPremiosLoteriaNavidad).");

            return new OkObjectResult(null);
        }
    }
}
