using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using Newtonsoft.Json;
using FluentAssertions;
using FeatureFlag.Shared.Models;
using cts = FeatureFlag.Shared.Constants;

namespace FeatureFlagApi6.Tests.Controllers
{
    public class FeaturesControllerTests
    {
        public class PostEvaluate : FeaturesControllerTests
        {

            /// <remarks>
            /// I know I'm breaking best practices here by putting so much
            /// testing into a single test.
            /// </remarks>
            [Fact]
            public async Task Basic_functionality_should_work()
            {
                var lambdaFunction = new LambdaEntryPoint();

                var requestStr = File.ReadAllText("./SampleRequests/FeaturesController-Evaluate-Post.json");
                var request = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(requestStr);
                var context = new TestLambdaContext();
                var response = await lambdaFunction.FunctionHandlerAsync(request, context);

                response.StatusCode.Should().Be(200);
                var featureList = JsonConvert.DeserializeObject<EvaluationResponse>(response.Body);
                featureList.Features.Should().NotBeNullOrEmpty();
                featureList.Features.Count().Should().Be(5);

                AssertFeatureResult(
                    featureList,
                    cts.TestValues.FeatureNames.NOT_DEFINED,
                    cts.Common.THIS_FEATURE_IS_OFF);

                AssertFeatureResult(
                    featureList,
                    cts.TestValues.FeatureNames.ALWAYS_ON,
                    cts.Common.THIS_FEATURE_IS_ON);

                AssertFeatureResult(
                    featureList,
                    cts.TestValues.FeatureNames.ALWAYS_OFF,
                    cts.Common.THIS_FEATURE_IS_OFF);

                AssertFeatureResult(
                    featureList,
                    cts.TestValues.FeatureNames.JWT_EMAIL_PARSE,
                    cts.Common.THIS_FEATURE_IS_ON);

                AssertFeatureResult(
                    featureList,
                    cts.TestValues.FeatureNames.JWT_EMAIL_PARSE_AND_ENVIRONMENT_HEADER,
                    cts.Common.THIS_FEATURE_IS_ON);

            }

            private void AssertFeatureResult(EvaluationResponse featureList, string featureName, bool expectedResult)
            {
                var theFeature = featureList.Features.First(o => o.Name == featureName);
                theFeature.IsOn.Should().Be(expectedResult);
            }
        }

    }
}
