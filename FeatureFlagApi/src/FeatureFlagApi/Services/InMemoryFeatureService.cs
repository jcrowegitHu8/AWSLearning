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
                Name="Module1_BillingReportOffAlwaysV2",
                Rules = new List<Rule>{ new Rule { Type = ruleType.boolean, Meta="false"} } },
            new Feature{
                Name="Module1_BillingReportOnAlwaysV2",
                Rules = new List<Rule>{ new Rule { Type = ruleType.boolean, Meta="true"} } },
            new Feature{ Name="Module2_OnForSpecificUsers",
                Rules = new List<Rule>{
                    new Rule { Type = ruleType.jwtParseMatchInList, Meta= "{ \"Path\":\"$.email\", \"List\":\"testuser1@example.com,testuser2@example.com\"}", }
                },
            },
            new Feature{ Name="Module2_OnForSpecificUsersForAnEnvironment",
                Rules = new List<Rule>{
                    new Rule { Type = ruleType.jwtParseMatchInList, Meta= "{ \"Path\":\"$.email\", \"List\":\"testuser1@example.com,testuser2@example.com\"}", },
                    new Rule { Type = ruleType.httpRequestHeaderExactMatch, Meta="{ \"Header\":\"x-env\", \"Value\":\"prod\" }", }
                }
            },
        };
    }
}
