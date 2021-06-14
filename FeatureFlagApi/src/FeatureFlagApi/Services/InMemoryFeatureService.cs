using FeatureFlagApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Services
{
    public class InMemoryFeatureService
    {
        internal static readonly List<Feature> _features = new List<Feature>
        {
            new Feature{
                Name=Constants.TestValues.FeatureNames.ALWAYS_OFF,
                Rules = new List<Rule>{ new Rule { Type = ruleType.boolean, Meta="false"} } },
            new Feature{
                Name=Constants.TestValues.FeatureNames.ALWAYS_ON,
                Rules = new List<Rule>{ new Rule { Type = ruleType.boolean, Meta="true"} } },
            new Feature{ Name=Constants.TestValues.FeatureNames.JWT_EMAIL_PARSE,
                Rules = new List<Rule>{
                    new Rule { Type = ruleType.jwtPayloadClaimMatchesValueInList, Meta= "{ \"Path\":\"$.email\", \"List\":\"testuser1@example.com,testuser2@example.com,johndoe@example.com\"}", }
                },
            },
            new Feature{ Name=Constants.TestValues.FeatureNames.JWT_EMAIL_PARSE_AND_ENVIRONMENT_HEADER,
                Rules = new List<Rule>{
                    new Rule { Type = ruleType.jwtPayloadClaimMatchesValueInList, Meta= "{ \"Path\":\"$.email\", \"List\":\"testuser1@example.com,testuser2@example.com,johndoe@example.com\"}", },
                    new Rule { Type = ruleType.httpRequestHeaderExactMatch, Meta="{ \"Header\":\"x-env\", \"Value\":\"prod\" }", }
                }
            },
        };
    }
}
