using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public class RuleReference : AbstractPattern
    {
        public Parser Parser { get; }
        public string RuleKey { get; }

        public RuleReference(Parser parser, string ruleKey)
        {
            Parser = parser;
            RuleKey = ruleKey;
        }

        public override PatternMatchResult Parse(ParsePosition parsePosition, ParseErrorCollector errorCollector)
        {
            var result = Parser.ParseRule(RuleKey, parsePosition, errorCollector);
            var patternMatchResult = result.PatternMatchResult;

            if (!patternMatchResult.Success)
            {
                //Debug.WriteLine($"RuleRef = {RuleKey}, fail={patternMatchResult}");
                return errorCollector.Add(PatternMatchResult.Failed(parsePosition).SetInnerResult(patternMatchResult));
            }

            if(!result.ProcessedResult.Success)
            {
                return errorCollector.Add(PatternMatchResult.Failed(result.ProcessedResult.ErrorPosition)
                    .SetErrorMessage(result.ProcessedResult.ErrorMessage))
                    .SetErrorPriority(1);
            }

            //Debug.WriteLine($"RuleRef = {RuleKey}, result={result.ProcessedResult.Value}, pos={patternMatchResult.ParsePosition.Index}");

            return PatternMatchResult.Succeeded(patternMatchResult.ParsePosition, result.ProcessedResult.Value).SetLabel(Label);
        }
    }
}
