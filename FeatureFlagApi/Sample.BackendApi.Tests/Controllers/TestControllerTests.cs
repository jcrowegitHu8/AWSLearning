using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Sample.BackendApi.Tests
{
    public class TestControllerTests: TestControllerTestBase , IClassFixture<WebApplicationFactory<Sample.BackendApi.Startup>>
    {
        public TestControllerTests(WebApplicationFactory<Sample.BackendApi.Startup> fixture) 
            :base(fixture)
        {

        }

        //[Fact]
        //public async Task Test_GET_Sync_should_work()
        //{
        //    var response = await this.Client.GetAsync("/test/sync");
        //    response.IsSuccessStatusCode.Should().BeTrue();
        //}

        //[Fact]
        //public async Task Test_GET_Aync_should_work()
        //{
        //    var response = await this.Client.GetAsync("/test/async");
        //    response.IsSuccessStatusCode.Should().BeTrue();
        //}

    }

    public class TestControllerTestBase
    {
        
        public HttpClient Client { get; }

        public TestControllerTestBase(WebApplicationFactory<Sample.BackendApi.Startup> fixture)
        {
            Client = fixture.CreateClient();
        }

        public TestControllerTestBase BuildForIntegration()
        {
            return this;
        }
    }

    //public class HttpSimpleClient
    //{
    //    public HttpClient _Client;
    //    public HttpSimpleClient(HttpClient client)
    //    {
    //        _Client = client;
    //    }

    //    public async Task<> Get(string url)
    //    {
    //        _Client.GetAsync
    //    }

    //}

}
