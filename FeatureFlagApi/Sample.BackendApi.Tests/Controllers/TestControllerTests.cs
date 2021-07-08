using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Sample.BackendApi.Tests
{
    public class TestControllerTests: TestControllerTestBase
    {
        public TestControllerTests()
        {

        }

        [Fact]
        public async Task Test_GET_Sync_should_work()
        {
            var response = await this.Client.GetAsync("/test/sync");
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task Test_GET_Aync_should_work()
        {
            var response = await this.Client.GetAsync("/test/async");
            response.IsSuccessStatusCode.Should().BeTrue();
        }

    }

    public class TestControllerTestBase
    {
        private IConfiguration _config;


        public HttpClient Client { get; }

        public TestControllerTestBase()
        {
            var config = Extensions.Configuration.GetIConfigurationRoot();
            var url = config.GetValue<string>("SampleBackendApiUrl");
            Client = new HttpClient();
            Client.BaseAddress = new Uri(url);
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
