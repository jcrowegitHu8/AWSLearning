using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.SDK.Models
{

    public class EvaluationRequest
    {
        public EvaluationRequest()
        {
            Features = new List<string>();
        }
        /// <summary>
        /// The list of features you want the api to tell you if they are ON/OFF
        /// </summary>
        public List<string> Features { get; set; }
    }

}
