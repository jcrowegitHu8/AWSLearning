using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Model
{
    /// <summary>
    /// A feature that has searchable key value pairs
    /// </summary>
    public class TaggedFeature
    {
        public string Name { get; set; }
        public bool DefaultEvaluation { get; set; }

        public IEnumerable<SearchableTag> Tags { get; set; }
    }
}
