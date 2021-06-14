using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Constants
{
    public class TestValues
    {
        public class FeatureNames
        {
            public const string NOT_DEFINED = "Sample_FeatureNotDefinedInDataStore";
            public const string ALWAYS_ON = "Sample_AlwaysOn";
            public const string ALWAYS_OFF = "Sample_AlwaysOff";
            public const string JWT_EMAIL_PARSE = "Sample_JwtEmailParse";
            public const string JWT_EMAIL_PARSE_AND_ENVIRONMENT_HEADER = "Sample_JwtEmailParseAndHeaderParse_MultipleRules";
        }
    }
}
