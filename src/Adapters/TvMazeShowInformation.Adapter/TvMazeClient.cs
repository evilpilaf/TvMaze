using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime.Extensions;
using Scraper.Core.Adapters;
using Scraper.Core.DomainExceptions;
using Scraper.Core.Entities;
using Scraper.Core.ValueTypes;

namespace TvMazeShowInformation.Adapter
{
    internal sealed class TvMazeClient : ITvMazeClient
    {
        private readonly HttpClient _client;

        private HttpClient CreateClient(IHttpClientFactory clientFactory)
        {
            var client = clientFactory.CreateClient(nameof(TvMazeClient));
            return client;
        }

        public TvMazeClient(IHttpClientFactory clientFactory)
        {
            _client = CreateClient(clientFactory);
        }

        public async Task<Result<Show>> GetShowById(int showId)
        {
            string uri = $"shows/{showId}";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<ShowDto>(contentString);
                var cast = await GetShowCast(showId);
                return cast.Match<IEnumerable<CastMember>, Result<Show>>(succ: castDtos => new Show(Guid.NewGuid(),
                        content.Name,
                        showId,
                        castDtos),
                    fail: ex => ex);
            }
            else
            {
                return HandleUnsuccessfulResponse(response);
            }
        }

        private async Task<Result<IEnumerable<CastMember>>> GetShowCast(int showId)
        {
            string uri = $"shows/{showId}/cast";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                var json = JArray.Parse(contentString);

                var persons = json.SelectTokens("$..person").Select(p => new CastMember
                (
                    id: p["id"].Value<int>(),
                    name: p["name"].Value<string>(),
                    birthDate: p["birthday"].Value<DateTime>().ToLocalDateTime().Date
                ));
                return new Result<IEnumerable<CastMember>>(persons);
            }
            else
            {
                return HandleUnsuccessfulResponse(response);
            }
        }

        private static Exception HandleUnsuccessfulResponse(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new NotFoundException();
                case (HttpStatusCode)429:
                    return new ThrottleException();
                default:
                    return new ApiCallException();
            }
        }
    }
}
