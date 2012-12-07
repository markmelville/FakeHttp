using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeHttp.Exceptions;

namespace FakeHttp
{
    public class InterceptingMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _handler;

        public InterceptingMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    var response = _handler(request, cancellationToken);
                    if (response == null)
                        throw new NullResponseException("The fake tried to return a null response.");
                    return response;
                });
        }
    }
}
