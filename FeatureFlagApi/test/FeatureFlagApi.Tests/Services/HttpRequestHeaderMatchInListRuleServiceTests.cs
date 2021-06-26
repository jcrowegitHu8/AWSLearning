using FakeItEasy;
using FeatureFlagApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FeatureFlagApi.Tests.Services
{
    public class HttpRequestHeaderMatchInListRuleServiceTests
    {
        public class Run : HttpRequestHeaderMatchInListRuleServiceTestBase
        {

            private const string VALID_META = "{ \"Header\":\"x-env\", \"List\":\"Dev,QA,Stage,Prod\"}";
            private const string VALID_META_LIST_CASE_SENSATIVE = "{ \"Header\":\"x-env\", \"List\":\"Dev,QA,Stage,Prod\"}";
            private const string VALID_META_NO_MATCH = "{ \"Header\":\"x-env\", \"List\":\"Dev,QA\"}";

            private const string META_EMPTY_LIST = "{ \"Header\":\"x-env\", \"List\":\"\"}";
            private const string META_NULL_LIST = "{ \"Header\":\"x-env\"}";

            [Fact]
            public void Should_return_true_for_a_valid_meta()
            {
                this.Build()
                    .WithProdHeader();
                var featureRuleResult = this.Target.Run(VALID_META);
                featureRuleResult.Should().BeTrue();
            }

            [Fact]
            public void Should_return_true_for_a_valid_meta_with_different_casing()
            {
                this.Build()
                    .WithProdHeader();
                var featureRuleResult = this.Target.Run(VALID_META_LIST_CASE_SENSATIVE);
                featureRuleResult.Should().BeTrue();
            }

            [Fact]
            public void Should_return_false_for_unable_to_read_token()
            {
                this.Build().WithEmptyHeader();
                var featureRuleResult = this.Target.Run(VALID_META);
                featureRuleResult.Should().BeFalse();
            }

            [Fact]
            public void Should_return_false_for_garbage_token_that_throws_an_exception()
            {
                this.Build().WithUnKownHeader();
                var featureRuleResult = this.Target.Run(VALID_META);
                featureRuleResult.Should().BeFalse();
            }

            [Fact]
            public void Should_return_false_for_a_no_match_token()
            {
                this.Build().WithProdHeader();
                var featureRuleResult = this.Target.Run(VALID_META_NO_MATCH);
                featureRuleResult.Should().BeFalse();
            }

            [Fact]
            public void Should_return_false_for_null_meta()
            {
                this.Build();
                var featureRuleResult = this.Target.Run(null);
                featureRuleResult.Should().BeFalse();
            }


            [Fact]
            public void Should_return_fales_for_an_empty_list()
            {
                this.Build()
                    .WithProdHeader();
                var featureRuleResult = this.Target.Run(META_EMPTY_LIST);
                featureRuleResult.Should().BeFalse();
            }

            [Fact]
            public void Should_return_fales_for_null_list()
            {
                this.Build()
                    .WithProdHeader();
                var featureRuleResult = this.Target.Run(META_NULL_LIST);
                featureRuleResult.Should().BeFalse();
            }
        }
    }

    public class HttpRequestHeaderMatchInListRuleServiceTestBase
    {
        protected HttpRequestHeaderMatchInListRuleService Target { get; set; }


        protected IRequestHeaderService HttpAccessorMock { get; set; }


        public HttpRequestHeaderMatchInListRuleServiceTestBase()
        {
            //Test Initialize
            HttpAccessorMock = A.Fake<IRequestHeaderService>();
        }

        private const string PROD_ENV = "Prod";

        public HttpRequestHeaderMatchInListRuleServiceTestBase Build()
        {
            this.Target = new HttpRequestHeaderMatchInListRuleService(HttpAccessorMock);
            return this;
        }

        public HttpRequestHeaderMatchInListRuleServiceTestBase WithProdHeader()
        {
            A.CallTo(() => HttpAccessorMock.GetFirstNotNullOrWhitespaceValue(A<string>.Ignored))
                .Returns(PROD_ENV);

            return this;
        }

        public HttpRequestHeaderMatchInListRuleServiceTestBase WithEmptyHeader()
        {

            A.CallTo(() => HttpAccessorMock.GetFirstNotNullOrWhitespaceValue(A<string>.Ignored))
                .Returns(string.Empty);

            return this;
        }

        public HttpRequestHeaderMatchInListRuleServiceTestBase WithUnKownHeader()
        {

            A.CallTo(() => HttpAccessorMock.GetFirstNotNullOrWhitespaceValue(A<string>.Ignored))
                .Returns("NO_ENV");

            return this;
        }

    }

}
