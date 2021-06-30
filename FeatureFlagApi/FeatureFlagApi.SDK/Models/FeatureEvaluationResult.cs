using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.SDK.Models
{
    public class FeatureEvaluationResult
    {
        public string Name { get; set; }
        public bool IsOn { get; set; }
    }
}
