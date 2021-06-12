using FeatureFlagApi.Controllers.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.Services
{
    public interface IRulesEngineService
    {
        EvaluationResponse Run(EvaluationRequest input);
    }

    public class RulesEngineService : IRulesEngineService
    {
        private const bool DEFAULT_FOR_ANY_FEATURE_THAT_DOES_NOT_EXIST = false;
        public EvaluationResponse Run(EvaluationRequest input)
        {
            var result = new EvaluationResponse();
            if(input == null || input.Features == null || !input.Features.Any())
            {
                return result;
            }


            foreach(var requestedFeature in input.Features)
            {
                var ruleToRun = InMemoryFeatureService._features.FirstOrDefault(o => 
                o.Name.Equals(requestedFeature, StringComparison.InvariantCultureIgnoreCase));
                if(ruleToRun == null)
                {
                    result.Features.Add(new Model.FeatureEvaluationResult
                    {
                        Name = requestedFeature,
                        IsOn = DEFAULT_FOR_ANY_FEATURE_THAT_DOES_NOT_EXIST
                    });
                    continue;
                }

                var rulesResultOfIsFeatureOn = RunAllRules(ruleToRun.Rules);
                result.Features.Add(new Model.FeatureEvaluationResult
                {
                    Name = requestedFeature,
                    IsOn = rulesResultOfIsFeatureOn
                });
            }

            return result;
        }

        public bool RunAllRules(List<Model.Rule> rules)
        {
            var theFeatureIsOff = false;
            var theFeatureIsOn = true;

            foreach(var rule in rules)
            {
                switch(rule.Type)
                {
                    case Model.ruleType.boolean:
                        if(!Boolean.Parse(rule.Meta))
                        {
                            return theFeatureIsOff;
                        }
                        else
                        {
                            return theFeatureIsOn;
                        }
                        break;
                }
            }

            return theFeatureIsOff;
        }

    }

}
