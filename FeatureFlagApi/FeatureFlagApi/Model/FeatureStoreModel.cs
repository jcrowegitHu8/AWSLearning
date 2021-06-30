using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Model
{
    public class FeatureStoreModel
    {
        /// <summary>
        /// An indicator of the version of the information in the feature store.
        /// You could make this a semantic version 1.0.2 
        /// OR modify this code to create a hash.  It's just a way to help
        /// troubleshoot things.
        /// </summary>
        public string Version { get; set; }

        public List<Feature> Features { get; set; }
    }
}
