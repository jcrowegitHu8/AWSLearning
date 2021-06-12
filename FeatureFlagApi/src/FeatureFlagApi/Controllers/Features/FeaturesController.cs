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
        private static readonly List<TaggedFeature> _features = new List<TaggedFeature>
        {
            new TaggedFeature{ Name="BillingReportOffAlwaysV2" },
            new TaggedFeature{ Name="BillingReportOnAlwaysV2", DefaultEvaluation=true },
            new TaggedFeature{ Name="OnForSpecificUsers",
                Tags = new List<SearchableTag>
                {
                    new SearchableTag { Key="Users", Value="testuser1@example.com,testuser2@example.com"}
                }
            },
            new TaggedFeature{ Name="OnForSpecificUsersForAnApplication",
                Tags = new List<SearchableTag>
                {
                    new SearchableTag { Key="Users", Value="testuser1@example.com,testuser2@example.com"},
                    new SearchableTag { Key="Application", Value="AngularApp1"}
                }
            },
        };
        // GET api/values
        [HttpGet]
        public IEnumerable<TaggedFeature> Get()
        {
            return _features;
        }

        [Route("BasicSearch")]
        [HttpGet]
        public IEnumerable<TaggedFeature> BasicSearch(string tag, matching match)
        {
            var results = new List<TaggedFeature>();
            switch (match)
            {
                case matching.exact:
                    var exactMatches = _features.Where(o =>
                        o.Tags != null && 
                        o.Tags.Any(t => t.Key.Equals(tag))).ToList();
                    results.AddRange(exactMatches);
                    break;
                case matching.startsWith:
                    var startsWithMatches = _features.Where(o => 
                        o.Tags !=null && 
                        o.Tags.Any(t => t.Key.StartsWith(tag, StringComparison.InvariantCultureIgnoreCase))).ToList();
                    results.AddRange(startsWithMatches);
                    break;
                case matching.contains:
                    var containsMatches = _features.Where(o =>
                        o.Tags != null && 
                        o.Tags.Any(t => t.Key.Contains(tag, StringComparison.InvariantCultureIgnoreCase))).ToList();
                    results.AddRange(containsMatches);
                    break;
            }

            return results;
        }

        [Route("AdvancedSearch")]
        [HttpPost]
        public IEnumerable<TaggedFeature> AdvancedSearch(SearchRequest search)
        {
            var results = new List<TaggedFeature>();
            foreach (var pattern in search.tags)
            {
                switch (pattern.Match)
                {
                    case matching.exact:
                        var exactMatches = _features.Where(o => o.Tags.Any(t => t.Key.Equals(pattern.Key))).ToList();
                        results.AddRange(exactMatches);
                        break;

                }

            }
            return results;
        }
    }
}
