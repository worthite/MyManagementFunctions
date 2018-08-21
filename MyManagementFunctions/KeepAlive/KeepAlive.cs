
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace MyManagementFunctions.Diagnostics
{
    public static class KeepAlive
    {
        [FunctionName("KeepAlive")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            string result = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            return result != null
                ? (ActionResult)new OkObjectResult($"{result}")
                : new BadRequestObjectResult("Invalid Response Time");
        }
    }
}
