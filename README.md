FakeHttp - because there a lot of real HTTP going on, and we need to test it.
=======================================================================
FakeHttp containers helpers that allow isolated testing of code that makes HTTP requests. Even though your code thinks it has a real object, all the responses are actually faked and so no actual HTTP request is made.

## HttpClient
The only currently supported object is System.Net.Http.HttpClient, which is new as of .NET 4.0. If this object implemented an interface, you could use your favorite mcok library as a fake. Since it doesn't, FakeHttp provides a FakeHttpClient which can be configured to return the desried responses.

    var fakeClient = new FakeHttpClient();
    fakeClient.AddRule(r => r.RequestUri.Host == "www.google.com", new HttpResponseMessage(HttpStatusCode.OK));
	
	var response = await fakeClient.GetAsync("http://www.google.com");
	Assert.AreEqual(HttpStatusCode.OK, response200.StatusCode);
	
## Contribute
To compile the project, you'll need Visual Studio 2012.

If you find a bug, create an issue in the Issue tracker (https://github.com/Ancestry/FakeHttp), create a unit test to validate the bug, then fix it.