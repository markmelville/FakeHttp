FakeHttp - because there's a lot of real HTTP going on, and we need to test it.
=======================================================================
FakeHttp contains helpers that allow isolated testing of code that makes HTTP requests. Even though the code thinks it has a real object, all the responses are faked and so no actual HTTP request is made.

## NuGet
This project is foun on NuGet.org at [https://nuget.org/packages/FakeHttp]

## HttpClient
The only currently supported object is System.Net.Http.HttpClient, which is new as of .NET 4.0. If this object implemented an interface, you could use your favorite mock library as a fake. Since it doesn't, we provide a FakeHttpClient which can be configured to return the desired responses.

    HttpClient client = new FakeHttpClient();
    client.AddRule(r => r.RequestUri.Host == "www.google.com", new HttpResponseMessage(HttpStatusCode.OK));
	
	var response = await client.GetAsync("http://www.google.com");
	Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
	
## Source Code
To compile the project, you'll need Visual Studio 2012.

If you find a bug, please create an issue in the Issue tracker (https://github.com/markmelville/FakeHttp/issues).