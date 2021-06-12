using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Model
{
    public class SearchableTag
    {
        public string Key { get; set; }
        public string Value { get; set; }

        //Todo make castable later
        //TODO: expand to a rules engine perhaps?
    }

}
