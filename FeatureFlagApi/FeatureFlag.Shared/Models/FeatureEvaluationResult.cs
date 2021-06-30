using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlag.Shared.Models
{
    public class FeatureEvaluationResult
    {
        public string Name { get; set; }
        public bool IsOn { get; set; }
    }
}
