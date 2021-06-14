using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Model
{
    public class EvaluationResponse
    {
        public EvaluationResponse()
        {
            Features = new List<FeatureEvaluationResult>();
        }
        public List<FeatureEvaluationResult> Features { get; set; }
    }
}
