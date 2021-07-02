using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FeatureFlagApi.SDK.Tests
{
    public class EvaluationClientTests : EvaluationClientTestBase, IClassFixture<WebApplicationFactory<FeatureFlagApi.Startup>>
    {
        public HttpClient Client { get; }

        public EvaluationClientTests(WebApplicationFactory<FeatureFlagApi.Startup> fixture)
        {
            Client = fixture.CreateClient();
        }

        [Fact]
        public void Thread_safty_test()
        {
            this.BuildForIntegration(Client);
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
        public void A_Valid_On_feature_should_return_true()
        {
            this.BuildForIntegration(Client);
            
            var result = Target.FeatureIsOn("Sample_AlwaysOn");
            result.Should().BeTrue();
        }

        [Fact]
        public void An_UnDefined_feature_should_return_false()
        {
            this.BuildForIntegration(Client);

            var result = Target.FeatureIsOn("Sample_NotAFeature");
            result.Should().BeFalse();
        }

        [Fact]
        public void A_Null_feature_should_return_false()
        {
            this.BuildForIntegration(Client);

            var result = Target.FeatureIsOn(null);
            result.Should().BeFalse();
        }

        [Fact]
        public void An_Empty_String_feature_should_return_false()
        {
            this.BuildForIntegration(Client);

            var result = Target.FeatureIsOn(string.Empty);
            result.Should().BeFalse();
        }
    }

    public class EvaluationClientTestBase 
    {
        public FeatureFlagService Target { get; set; }

        public EvaluationClientTestBase()
        {
        }

        public EvaluationClientTestBase BuildForIntegration(HttpClient inMemoryApiClient)
        {
            var options = new FeatureFlagSDKOptions
            {
                FeaturesToTrack = new List<string> { "Sample_AlwaysOn" },
                ApiBaseUrl = "/api/features"
            };

            this.Target = new FeatureFlagService(inMemoryApiClient, options);
            return this;
        }


        }
}
