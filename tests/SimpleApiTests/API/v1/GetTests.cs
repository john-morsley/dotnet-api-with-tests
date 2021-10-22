using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using SimpleApi;

namespace SimpleApiTests.API.v1
{
    public class Tests
    {
        private TestServer _server;
        private HttpClient _client;
        
        [SetUp]
        public void Setup()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<StartUp>());
            _client = _server.CreateClient();
        }

        [Test]
        public async Task GetDefaultSwaggerWithIncompleteUrl()
        {
            // Arrange...
            
            // Act...
            var httpResponse = await _client.GetAsync("/");
            
            // Assert...
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Moved);
            var response = await httpResponse.Content.ReadAsStringAsync();
            response.Length.Should().Be(0);
        }
        
        [Test]
        public async Task GetDefaultSwaggerWithCompleteUrl()
        {
            // Arrange...
            
            // Act...
            var httpResponse = await _client.GetAsync("/index.html");
            
            // Assert...
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await httpResponse.Content.ReadAsStringAsync();
            response.Length.Should().BeGreaterThan(0);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);
            var title = htmlDocument.DocumentNode.SelectSingleNode("//title").InnerText;
            title.Should().Be("Swagger UI");
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}