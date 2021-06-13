using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Model
{
    public class MetaHttpRequestHeaderExactMatch
    {
        public string Header { get; set; }
        public string Value { get; set; }
    }
}
