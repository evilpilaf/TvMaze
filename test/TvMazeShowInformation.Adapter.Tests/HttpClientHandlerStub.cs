using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TvMazeShowInformation.Adapter.Tests
{
    public sealed class HttpMessageHandlerStub : HttpMessageHandler
    {
        public Dictionary<string, HttpResponseMessage> EndpointResponses = new Dictionary<string, HttpResponseMessage>();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool isResponseSetup = EndpointResponses.TryGetValue(request.RequestUri.LocalPath, out var responseMessage);

            if (isResponseSetup)
            {
                return Task.FromResult(responseMessage);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotImplemented));
        }
    }
}
