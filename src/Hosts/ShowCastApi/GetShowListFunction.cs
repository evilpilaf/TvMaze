using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NodaTime;

using Scraper.Core.DomainExceptions;
using Scraper.Core.Entities;
using Scraper.Core.UseCases;
using Scraper.Core.ValueTypes;

namespace ShowCastApi
{
    public class GetShowListFunction
    {
        private readonly GetPageOfShowsUseCase _getPageOfShowsUseCase;

        public GetShowListFunction(GetPageOfShowsUseCase getPageOfShowsUseCase)
        {
            _getPageOfShowsUseCase = getPageOfShowsUseCase;
        }

        [FunctionName("GetShowListFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,
             "get",
             Route = "show")]
            HttpRequest req,
            ILogger log
            )
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int pageNumber = 1;
            int pageSize = 10;

            if (!string.IsNullOrWhiteSpace(req.Query["pageNumber"]))
            {
                pageNumber = int.Parse(req.Query["pageNumber"]);
            }

            if (!string.IsNullOrWhiteSpace(req.Query["pageSize"]))
            {
                pageSize = int.Parse(req.Query["pageSize"]);
            }

            var result = await _getPageOfShowsUseCase.Execute(pageSize, pageNumber);

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var b = new JsonSerializer();

            var actionResult = result.Match<PageResult<Show>, ActionResult>(
                succ: page => new JsonResult(page.Results, jsonSettings), 
                fail: ex =>
                {
                    switch (ex)
                    {
                        case NotFoundException _:
                            return new NotFoundResult();
                        default:
                            return new InternalServerErrorResult();
                    }
                });

            return (ActionResult)actionResult;
        }
    }
}
