using FeatureFlagApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlagApi.Extensions;
using YamlDotNet.Serialization;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace FeatureFlagApi.Services
{
    public class InMemoryFeatureService : IFeatureRepository
    {
        private static readonly FeatureStoreModel _features = new FeatureStoreModel
        {
            Version = "InMemoryFeatureService",
            Features = new List<Feature>
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
                        new Rule { Type = ruleType.httpRequestHeaderMatchInList, Meta="{ \"Header\":\"x-env\", \"List\":\"prod\" }", }
                    }
                },
            }
        };


        public FeatureStoreModel GetAll()
        {
            return _features;
        }
    }



    public interface IFeatureRepository
    {
        FeatureStoreModel GetAll();
    }
}
