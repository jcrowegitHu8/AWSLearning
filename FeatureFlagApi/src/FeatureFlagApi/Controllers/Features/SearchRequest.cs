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
        public IDictionary<string,string> tags { get; set; }
    }
}
