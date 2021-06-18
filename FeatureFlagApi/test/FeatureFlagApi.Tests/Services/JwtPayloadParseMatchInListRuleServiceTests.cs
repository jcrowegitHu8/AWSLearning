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
    public class JwtPayloadParseMatchInListRuleServiceTests
    {
        public class Run : JwtPayloadParseMatchInListRuleServiceTestBase
        {

            private const string VALID_META = "{ \"Path\":\"$.email\", \"List\":\"testuser1@example.com,testuser2@example.com,johndoe@example.com\"}";
            private const string VALID_META_LIST_CASE_SENSATIVE = "{ \"Path\":\"$.email\", \"List\":\"testuser1@example.com,testuser2@example.com,JohnDoe@example.com\"}";
            private const string VALID_META_NO_MATCH = "{ \"Path\":\"$.email\", \"List\":\"testuser1@example.com,testuser2@example.com\"}";

            private const string META_EMPTY_LIST = "{ \"Path\":\"$.email\", \"List\":\"\"}";
            private const string META_NULL_LIST = "{ \"Path\":\"$.email\"}";

            [Fact]
            public async Task Should_return_true_for_a_valid_meta()
            {
                this.Build()
                    .WithValidToken();
                var featureRuleResult = this.Target.Run(VALID_META);
                featureRuleResult.Should().BeTrue();
            }

            [Fact]
            public async Task Should_return_true_for_a_valid_meta_with_different_casing()
            {
                this.Build()
                    .WithValidToken();
                var featureRuleResult = this.Target.Run(VALID_META_LIST_CASE_SENSATIVE);
                featureRuleResult.Should().BeTrue();
            }

            [Fact]
            public async Task Should_return_false_for_unable_to_read_token()
            {
                this.Build().WithUnableToReadToken();
                var featureRuleResult = this.Target.Run(VALID_META);
                featureRuleResult.Should().BeFalse();
            }

            [Fact]
            public async Task Should_return_false_for_garbage_token_that_throws_an_exception()
            {
                this.Build().WithUnableToReadToken();
                var featureRuleResult = this.Target.Run(VALID_META);
                featureRuleResult.Should().BeFalse();
            }

            [Fact]
            public async Task Should_return_false_for_a_no_match_token()
            {
                this.Build().WithValidToken();
                var featureRuleResult = this.Target.Run(VALID_META_NO_MATCH);
                featureRuleResult.Should().BeFalse();
            }

            [Fact]
            public async Task Should_return_false_for_null_meta()
            {
                this.Build();
                var featureRuleResult = this.Target.Run(null);
                featureRuleResult.Should().BeFalse();
            }


            [Fact]
            public async Task Should_return_fales_for_an_empty_list()
            {
                this.Build()
                    .WithValidToken();
                var featureRuleResult = this.Target.Run(META_EMPTY_LIST);
                featureRuleResult.Should().BeFalse();
            }

            [Fact]
            public async Task Should_return_fales_for_null_list()
            {
                this.Build()
                    .WithValidToken();
                var featureRuleResult = this.Target.Run(META_NULL_LIST);
                featureRuleResult.Should().BeFalse();
            }
        }
    }

    public class JwtPayloadParseMatchInListRuleServiceTestBase
    {
        protected JwtPayloadParseMatchInListRuleService Target { get; set; }


        protected IAuthHeaderService AuthHeaderServiceMock { get; set; }


        public JwtPayloadParseMatchInListRuleServiceTestBase()
        {
            Initialize();
        }
        public void Initialize()
        {
            AuthHeaderServiceMock = A.Fake<IAuthHeaderService>();
        }

        private const string VALID_JWT_TOKEN_ONLY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJlbWFpbCI6ImpvaG5kb2VAZXhhbXBsZS5jb20ifQ.d6Tv2Xy6UUYdSpTQAKEj3mZJF5Q14OdlT3BbUyuZiuU";
        private const string UNABLE_TO_READ_JWT_TOKEN_ONLY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJlbWFpbCI6ImpvaG5kb2VAZXhhbXBsZS5jb20ifQd6Tv2Xy6UUYdSpTQAKEj3mZJF5Q14OdlT3BbUyuZiuU";
        private const string GARBAGE_JWT_TOKEN_ONLY = "JhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJlbWFpbCI6ImpvaG5kb2VAZXhhbXBsZS5jb20ifQ.d6Tv2Xy6UUYdSpTQAKEj3mZJF5Q14OdlT3BbUyuZiuU";


        public JwtPayloadParseMatchInListRuleServiceTestBase Build()
        {
            this.Target = new JwtPayloadParseMatchInListRuleService(AuthHeaderServiceMock);
            return this;
        }

        public JwtPayloadParseMatchInListRuleServiceTestBase WithValidToken()
        {

            A.CallTo(() => AuthHeaderServiceMock.GetTokenOnly())
                .Returns(VALID_JWT_TOKEN_ONLY);

            return this;
        }

        public JwtPayloadParseMatchInListRuleServiceTestBase WithUnableToReadToken()
        {

            A.CallTo(() => AuthHeaderServiceMock.GetTokenOnly())
                .Returns(UNABLE_TO_READ_JWT_TOKEN_ONLY);

            return this;
        }

        public JwtPayloadParseMatchInListRuleServiceTestBase WithGarbageToken()
        {

            A.CallTo(() => AuthHeaderServiceMock.GetTokenOnly())
                .Returns(UNABLE_TO_READ_JWT_TOKEN_ONLY);

            return this;
        }

    }

}
