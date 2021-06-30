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
using FeatureFlagApi.Model;
using FluentAssertions;

namespace FeatureFlagApi.Tests.Controllers
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
                    Constants.TestValues.FeatureNames.NOT_DEFINED,
                    Constants.Common.THIS_FEATURE_IS_OFF);

                AssertFeatureResult(
                    featureList,
                    Constants.TestValues.FeatureNames.ALWAYS_ON,
                    Constants.Common.THIS_FEATURE_IS_ON);

                AssertFeatureResult(
                    featureList,
                    Constants.TestValues.FeatureNames.ALWAYS_OFF,
                    Constants.Common.THIS_FEATURE_IS_OFF);

                AssertFeatureResult(
                    featureList,
                    Constants.TestValues.FeatureNames.JWT_EMAIL_PARSE,
                    Constants.Common.THIS_FEATURE_IS_ON);

                AssertFeatureResult(
                    featureList,
                    Constants.TestValues.FeatureNames.JWT_EMAIL_PARSE_AND_ENVIRONMENT_HEADER,
                    Constants.Common.THIS_FEATURE_IS_ON);

            }

            private void AssertFeatureResult(EvaluationResponse featureList, string featureName, bool expectedResult)
            {
                var theFeature = featureList.Features.First(o => o.Name == featureName);
                theFeature.IsOn.Should().Be(expectedResult);
            }
        }

    }
}
