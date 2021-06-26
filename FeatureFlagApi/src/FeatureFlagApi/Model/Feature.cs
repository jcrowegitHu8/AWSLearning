using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Model
{
    public class Feature
    {
        public string Name { get; set; }
        
        public List<Rule> Rules { get; set; }
    }

    public class Rule
    {
        [JsonConverter(typeof(StringEnumConverter))]

        public ruleType Type { get; set; }
        /// <summary>
        /// Either a simple value or a JSON object to allow the rule to be evaluated
        /// </summary>
        public string Meta { get; set; }
    }

    public enum ruleType
    {
        /// <summary>
        /// The rule value just specifies on/off
        /// </summary>
        undefined = -1,
        boolean = 0,
        jwtPayloadClaimMatchesValueInList = 1,
        httpRequestHeaderExactMatch=2
    }

    public class JwtParseMatchInList
    {
        /// <summary>
        /// The JSON path to run against a decripted standard JWT
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The comma delimited list of value that are acceptable exact matches
        /// </summary>
        public string List { get; set; }

        //TODO: add delimiter property to make more configurable.
    }
}
