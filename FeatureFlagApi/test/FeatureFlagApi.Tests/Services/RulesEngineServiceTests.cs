using FakeItEasy;
using FeatureFlagApi.Model;
using FeatureFlagApi.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FeatureFlagApi.Tests.Services
{
    public class RulesEngineServiceTests
    {
        public class Run : RulesEngineServiceTestsBase
        {
            private const string VALID_META = "{ \"Path\":\"$.email\", \"List\":\"testuser1@example.com,testuser2@example.com,johndoe@example.com\"}";


            [Fact]
            public void Should_not_return_null_for_null_input()
            {
                this.Build();
                var evaluationResponse = this.Target.Run(null);
                evaluationResponse.Should().NotBeNull();
                evaluationResponse.Features.Should().NotBeNull();
            }

            [Fact]
            public void Should_not_return_null_for_feature_list_input_being_null()
            {
                this.Build();
                var input = new EvaluationRequest() { Features = null };
                var evaluationResponse = this.Target.Run(input);
                evaluationResponse.Should().NotBeNull();
                evaluationResponse.Features.Should().NotBeNull();
            }

            [Fact]
            public void Should_not_return_null_for_feature_list_input_being_empty()
            {
                this.Build();
                var input = new EvaluationRequest();
                var evaluationResponse = this.Target.Run(input);
                evaluationResponse.Should().NotBeNull();
                evaluationResponse.Features.Should().NotBeNull();
            }

            [Fact]
            public void Should_return_false_for_a_not_defined_feature()
            {
                this.Build();
                var input = new EvaluationRequest() { Features = new List<string> { "NotDefinedFeature" } };
                var evaluationResponse = this.Target.Run(input);
                evaluationResponse.Features.First().IsOn.Should().BeFalse();
            }

            [Fact]
            public void Should_return_false_for_a_defined_rule_with_an_invalid_rule_type()
            {
                this.Build();
                var input = new EvaluationRequest() { Features = new List<string> { INVALID_RULE_TYPE_FEATURE } };
                var evaluationResponse = this.Target.Run(input);
                evaluationResponse.Features.First().IsOn.Should().BeFalse();
            }
        }
    }

    public class RulesEngineServiceTestsBase
    {
        protected RulesEngineService Target { get; set; }

        protected IFeatureRepository FeatureRepositoryMock { get; set; }
        protected IHttpRequestHeaderMatchInListRuleService HttpRequestHeaderMatchInListRuleServiceMock { get; set; }
        protected IJwtPayloadParseMatchInListRuleService JwtPayloadParseMatchInListRuleServiceMock { get; set; }

        protected FeatureStoreModel TestFeatures { get; set; }

        protected const string INVALID_RULE_TYPE_FEATURE = "INVALID_RULE_TYPE_FEATURE";

        public RulesEngineServiceTestsBase()
        {
            //Test Initialize

            //Todo just DI the actual YamlFeatureFlagStore
            TestFeatures = new FeatureStoreModel
            {
                Version = "UnitTestVersion",
                Features = new List<Feature> {
                new Feature { Name = INVALID_RULE_TYPE_FEATURE, Rules = new List<Rule> { new Rule { Type = ruleType.undefined } } }
            }
            };

            FeatureRepositoryMock = A.Fake<IFeatureRepository>();
            A.CallTo(() => FeatureRepositoryMock.GetAll()).Returns(TestFeatures);

            HttpRequestHeaderMatchInListRuleServiceMock = A.Fake<IHttpRequestHeaderMatchInListRuleService>();

            JwtPayloadParseMatchInListRuleServiceMock = A.Fake<IJwtPayloadParseMatchInListRuleService>();
        }

        public RulesEngineServiceTestsBase Build()
        {
            this.Target = new RulesEngineService(
                FeatureRepositoryMock,
                HttpRequestHeaderMatchInListRuleServiceMock,
                JwtPayloadParseMatchInListRuleServiceMock);
            return this;
        }

    }
}
