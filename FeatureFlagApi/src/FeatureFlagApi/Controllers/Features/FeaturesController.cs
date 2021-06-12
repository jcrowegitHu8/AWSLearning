using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlagApi.Controllers.Features;
using FeatureFlagApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagApi.Controllers
{
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private static readonly List<Feature> _features = new List<Feature>
        {
            new Feature{ Name="BillingReportV2", IsOn=false }
        };
        // GET api/values
        [HttpGet]
        public IEnumerable<Feature> Get()
        {
            return _features;
        }

        [Route("BasicSearch")]
        [HttpPost]
        public IEnumerable<Feature> BasicSearch(SearchRequest search)
        {
            return _features;
            //_features.Where(o => o.)
        }
    }
}
