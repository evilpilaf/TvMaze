using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;
using Scraper.Core.DomainExceptions;
using Scraper.Test.Utils;

using Xunit;

namespace TvMazeShowInformation.Adapter.Tests
{
    public class TvMazeClientTests
    {
        private readonly Mock<IHttpClientFactory> _clientFactoryMock;

        public TvMazeClientTests()
        {
            _clientFactoryMock = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task WhenApiCallSucceeds_AndContainsShowInformation_ReturnsShowEntity()
        {
            const int showId = 1;
            const string mockShowResponse =
                "{\"id\":1,\"url\":\"http://www.tvmaze.com/shows/1/under-the-dome\",\"name\":\"Under the Dome\",\"type\":\"Scripted\",\"language\":\"English\",\"genres\":[\"Drama\",\"Science-Fiction\",\"Thriller\"],\"status\":\"Ended\",\"runtime\":60,\"premiered\":\"2013-06-24\",\"officialSite\":\"http://www.cbs.com/shows/under-the-dome/\",\"schedule\":{\"time\":\"22:00\",\"days\":[\"Thursday\"]},\"rating\":{\"average\":6.5},\"weight\":92,\"network\":{\"id\":2,\"name\":\"CBS\",\"country\":{\"name\":\"United States\",\"code\":\"US\",\"timezone\":\"America/New_York\"}},\"webChannel\":null,\"externals\":{\"tvrage\":25988,\"thetvdb\":264492,\"imdb\":\"tt1553656\"},\"image\":{\"medium\":\"http://static.tvmaze.com/uploads/images/medium_portrait/81/202627.jpg\",\"original\":\"http://static.tvmaze.com/uploads/images/original_untouched/81/202627.jpg\"},\"summary\":\"<p><b>Under the Dome</b> is the story of a small town that is suddenly and inexplicably sealed off from the rest of the world by an enormous transparent dome. The town's inhabitants must deal with surviving the post-apocalyptic conditions while searching for answers about the dome, where it came from and if and when it will go away.</p>\",\"updated\":1562326291,\"_links\":{\"self\":{\"href\":\"http://api.tvmaze.com/shows/1\"},\"previousepisode\":{\"href\":\"http://api.tvmaze.com/episodes/185054\"}}}";
            const string mockClientResponse =
                @"[{""person"":{""id"":1,""url"":""http://www.tvmaze.com/people/1/mike-vogel"",""name"":""Mike Vogel"",""country"":{""name"":""United States"",""code"":""US"",""timezone"":""America/New_York""},""birthday"":""1979-07-17"",""deathday"":null,""gender"":""Male"",""image"":{""medium"":""http://static.tvmaze.com/uploads/images/medium_portrait/0/1815.jpg"",""original"":""http://static.tvmaze.com/uploads/images/original_untouched/0/1815.jpg""},""_links"":{""self"":{""href"":""http://api.tvmaze.com/people/1""}}},""character"":{""id"":1,""url"":""http://www.tvmaze.com/characters/1/under-the-dome-dale-barbie-barbara"",""name"":""Dale \""Barbie\"" Barbara"",""image"":{""medium"":""http://static.tvmaze.com/uploads/images/medium_portrait/0/3.jpg"",""original"":""http://static.tvmaze.com/uploads/images/original_untouched/0/3.jpg""},""_links"":{""self"":{""href"":""http://api.tvmaze.com/characters/1""}}},""self"":false,""voice"":false},{""person"":{""id"":2,""url"":""http://www.tvmaze.com/people/2/rachelle-lefevre"",""name"":""Rachelle Lefevre"",""country"":{""name"":""Canada"",""code"":""CA"",""timezone"":""America/Halifax""},""birthday"":""1979-02-01"",""deathday"":null,""gender"":""Female"",""image"":{""medium"":""http://static.tvmaze.com/uploads/images/medium_portrait/82/207417.jpg"",""original"":""http://static.tvmaze.com/uploads/images/original_untouched/82/207417.jpg""},""_links"":{""self"":{""href"":""http://api.tvmaze.com/people/2""}}},""character"":{""id"":2,""url"":""http://www.tvmaze.com/characters/2/under-the-dome-julia-shumway"",""name"":""Julia Shumway"",""image"":{""medium"":""http://static.tvmaze.com/uploads/images/medium_portrait/0/6.jpg"",""original"":""http://static.tvmaze.com/uploads/images/original_untouched/0/6.jpg""},""_links"":{""self"":{""href"":""http://api.tvmaze.com/characters/2""}}},""self"":false,""voice"":false}]";

            var mockShowResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockShowResponse)
            };

            var mockCastResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(mockClientResponse)
            };

            var handler = new HttpMessageHandlerStub();
            handler.EndpointResponses.Add($"/shows/{showId}", mockShowResponseMessage);
            handler.EndpointResponses.Add($"/shows/{showId}/cast", mockCastResponseMessage);

            var client = CreateClient(handler);

            _clientFactoryMock.Setup(m => m.CreateClient(It.IsAny<string>()))
                              .Returns(client);

            var sut = new TvMazeClient(_clientFactoryMock.Object);

            var result = await sut.GetShowById(showId);

            result.Should().BeSuccess();
        }

        [Fact]
        public async Task WhenRateLimitReached_ReturnsFailedResult_WithThrottledException()
        {
            const int showId = 1;
            var apiLimitResponseMessage = new HttpResponseMessage((HttpStatusCode)429);

            var handler = new HttpMessageHandlerStub();
            handler.EndpointResponses.Add($"/shows/{showId}", apiLimitResponseMessage);

            var client = CreateClient(handler);

            _clientFactoryMock.Setup(m => m.CreateClient(It.IsAny<string>()))
                              .Returns(client);

            var sut = new TvMazeClient(_clientFactoryMock.Object);

            var result = await sut.GetShowById(showId);

            result.Should().BeFailed().Which.Should().BeOfType<ThrottleException>();
        }

        private static HttpClient CreateClient(HttpMessageHandler messageHandler)
        {
            return new HttpClient(messageHandler) { BaseAddress = new Uri("http://dummyUri") };
        }
    }
}
