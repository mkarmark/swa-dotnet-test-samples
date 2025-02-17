using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace dotnet6_inproc
{
    public static class HttpTrigger
    {
        [FunctionName("HttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("GetCsv")]
        public static async Task<IActionResult> RunGetCsvAsync(
                    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("hello world!");
            writer.Flush();
            stream.Position = 0;
        
            return new FileStreamResult(stream, "text/csv")
            {
                FileDownloadName = $"Sample-{DateTime.Today.ToString("yyyy-MM-dd")}.csv",
            };
        }

        [FunctionName("GetCsv2")]
        public static async Task<IActionResult> RunGetCsvAsync2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello world!"));
            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = "somefile"
            };
        }
    }
}
