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


            [Fact]
            public async Task Should_return_false_for_null_meta()
            {
                this.Build();
                var featureRuleResult = this.Target.Run(null);
                featureRuleResult.Should().BeFalse();
            }

            [Fact]
            public async Task Should_return_true_for_a_valid_meta()
            {
                this.Build();
                var featureRuleResult = this.Target.Run(VALID_META);
                featureRuleResult.Should().BeTrue();
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
            A.CallTo(() => AuthHeaderServiceMock.GetTokenOnly())
                .Returns(VALID_JWT_TOKEN_ONLY);

        }

        private const string VALID_JWT_TOKEN_ONLY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJlbWFpbCI6ImpvaG5kb2VAZXhhbXBsZS5jb20ifQ.d6Tv2Xy6UUYdSpTQAKEj3mZJF5Q14OdlT3BbUyuZiuU";

        public void Build()
        {
            this.Target = new JwtPayloadParseMatchInListRuleService(AuthHeaderServiceMock);
        }
    }

}
