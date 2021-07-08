using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FeatureFlagApi.SDK.Tests
{
    public class EvaluationClientTests : EvaluationClientTestBase, IClassFixture<WebApplicationFactory<FeatureFlagApi.Startup>>
    {
        public HttpClient Client { get; }
        private readonly ITestOutputHelper _testOutputHelper;

        public EvaluationClientTests(WebApplicationFactory<FeatureFlagApi.Startup> fixture,
            ITestOutputHelper testOutputHelper)
        {
            Client = fixture.CreateClient();
            var url = Client.BaseAddress.AbsoluteUri.ToString() + "api/";
            Client.BaseAddress = new Uri(url);
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Thread_safty_test()
        {
            this.BuildForThreadSafeIntegration(Client, _testOutputHelper);
            var count = 20;
            Thread[] threads = new Thread[count];

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(WorkForMultiThreadedTest);
            }

            foreach (Thread thread in threads)
            {
                Thread.Sleep(1000);
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }


        private void WorkForMultiThreadedTest()
        {
            Target.FeatureIsOn("Sample_AlwaysOn");
        }

        [Fact]
        public void DoNothing_should_work_and_always_return_false()
        {
            this.BuildWithNoFeaturesIntegration(Client, _testOutputHelper);

            var result = Target.FeatureIsOn("Sample_AlwaysOn");
            result.Should().BeFalse();
        }

        [Fact]
        public void A_Valid_On_feature_should_return_true()
        {
            this.BuildForIntegration(Client, _testOutputHelper);

            var result = Target.FeatureIsOn("Sample_AlwaysOn");
            result.Should().BeTrue();
        }

        [Fact]
        public void An_UnDefined_feature_should_return_false()
        {
            this.BuildForIntegration(Client, _testOutputHelper);

            var result = Target.FeatureIsOn("Sample_NotAFeature");
            result.Should().BeFalse();
        }

        [Fact]
        public void A_Null_feature_should_return_false()
        {
            this.BuildForIntegration(Client, _testOutputHelper);

            var result = Target.FeatureIsOn(null);
            result.Should().BeFalse();
        }

        [Fact]
        public void An_Empty_String_feature_should_return_false()
        {
            this.BuildForIntegration(Client, _testOutputHelper);

            var result = Target.FeatureIsOn(string.Empty);
            result.Should().BeFalse();
        }
    }

    public class EvaluationClientTestBase
    {
        public FeatureFlagService Target { get; set; }

        public FeatureFlagSDKOptions options { get; set; }

        public EvaluationClientTestBase()
        {

        }

        public EvaluationClientTestBase BuildForIntegration(HttpClient inMemoryApiClient
            , ITestOutputHelper testOutputHelper)
        {
            var logger = XUnitLogger.CreateLogger<FeatureFlagService>(testOutputHelper);
            var options = new FeatureFlagSDKOptions
            {
                FeaturesToTrack = new List<string> { "Sample_AlwaysOn" },
                HttpClient = inMemoryApiClient,
                Logger = logger
            };

            this.Target = new FeatureFlagService(options);
            return this;
        }

        public EvaluationClientTestBase BuildForThreadSafeIntegration(HttpClient inMemoryApiClient,
            ITestOutputHelper testOutputHelper)
        {
            var logger = XUnitLogger.CreateLogger<FeatureFlagService>(testOutputHelper);
            var options = new FeatureFlagSDKOptions
            {
                FeaturesToTrack = new List<string> { "Sample_AlwaysOn" },
                RefreshInterval = TimeSpan.FromSeconds(5),
                HttpClient = inMemoryApiClient,
                Logger = logger
            };

            this.Target = new FeatureFlagService(options);
            return this;
        }

        public EvaluationClientTestBase BuildWithNoFeaturesIntegration(HttpClient inMemoryApiClient,
           ITestOutputHelper testOutputHelper)
        {
            var logger = XUnitLogger.CreateLogger<FeatureFlagService>(testOutputHelper);
            var options = new FeatureFlagSDKOptions
            {
                FeaturesToTrack = new List<string> { },
                RefreshInterval = TimeSpan.FromSeconds(5),
                HttpClient = inMemoryApiClient,
                Logger = logger
            };

            this.Target = new FeatureFlagService(options);
            return this;
        }


    }
}
