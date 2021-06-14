using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Model
{

    public class EvaluationRequest
    {
        /// <summary>
        /// The list of features you want the api to tell you if they are ON/OFF
        /// </summary>
        public List<string> Features { get; set; }
    }

    public enum matching
    {
        exact = 0,
        startsWith = 1,
        contains = 2,
        endsWith = 3
    }
}
