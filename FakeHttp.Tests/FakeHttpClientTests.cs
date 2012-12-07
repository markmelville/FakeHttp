using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeHttp.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FakeHttp.Tests
{
    [TestClass]
    public class FakeHttpClientTests
    {
        readonly static HttpResponseMessage OkResponse = new HttpResponseMessage(HttpStatusCode.OK);
        readonly static HttpResponseMessage NotFound = new HttpResponseMessage(HttpStatusCode.NotFound);
        readonly static HttpResponseMessage ErrorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        [TestMethod]
        [ExpectedException(typeof(FakeNotSetupException))]
        public async Task FakeHttpClient_ThrowsExceptionWhenFakeIsNotSetup()
        {
            var fakeClient = new FakeHttpClient();
            var response = await fakeClient.GetAsync("http://www.test.com");
        }

        [TestMethod]
        [ExpectedException(typeof(NullResponseException))]
        public async Task FakeHttpClient_ThrowsExceptionWhenFakeReturnsNull()
        {
            var fakeClient = new FakeHttpClient();
            fakeClient.AddRule(r => null);
            var response = await fakeClient.GetAsync("http://www.test.com");
        }

        [TestMethod]
        public async Task FakeHttpClient_AllowsRulesWithUnconditionalReturnValue()
        {
            var fakeClient = new FakeHttpClient();

            fakeClient.AddRule(r => r.RequestUri.Host == "www.200.com", OkResponse);
            fakeClient.AddRule(r => r.RequestUri.Host == "www.500.com", ErrorResponse);
            fakeClient.AddRule(r => r.RequestUri.Host == "www.404.com", NotFound);

            var response404 = await fakeClient.GetAsync("http://www.404.com");
            var response200 = await fakeClient.GetAsync("http://www.200.com");
            var response500 = await fakeClient.GetAsync("http://www.500.com");

            Assert.AreEqual(HttpStatusCode.NotFound, response404.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response200.StatusCode);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response500.StatusCode);
        }

        [TestMethod]
        public async Task FakeHttpClient_AllowsRulesWithConditionalReturnValue()
        {
            var fakeClient = new FakeHttpClient();

            fakeClient.AddRule(req => req.RequestUri.Host == "www.404.com" ? NotFound : OkResponse);

            var response404 = await fakeClient.GetAsync("http://www.404.com");
            var response200 = await fakeClient.GetAsync("http://www.200.com");

            Assert.AreEqual(HttpStatusCode.NotFound, response404.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response200.StatusCode);
        }
    }
}