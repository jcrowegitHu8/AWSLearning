using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Model
{
    public class FeatureEvaluationResult
    {
        public string Name { get; set; }
        public bool IsOn { get; set; }
    }
}
