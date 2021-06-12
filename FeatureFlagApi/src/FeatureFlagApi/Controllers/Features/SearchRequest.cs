using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Controllers.Features
{
    public class SearchRequest
    {
        /// <summary>
        /// Generic key value pairs that can be searched for
        /// </summary>
        /// <remarks>
        /// Examples:
        /// "Environment":"Prod"
        /// "Application":"AngularApp1"
        /// "Module":"Billing"
        /// </remarks>
        public IEnumerable<TagSearchPattern> tags { get; set; }
    }

    public class TagSearchPattern
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public matching Match { get; set; }
    }

    public enum matching
    {
        exact=0,
        startsWith=1,
        contains=2,
        endsWith=3
    }
}
