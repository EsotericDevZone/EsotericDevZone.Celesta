using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Parser.Patterns;
using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EsotericDevZone.Celesta.Parser
{
    public class Parser
    {
        public string MainRuleKey { get; set; }

        private readonly Dictionary<string, ParseRule> Rules = new Dictionary<string, ParseRule>();

        public class ParseRuleResult
        {
            public PatternMatchResult PatternMatchResult { get; }
            public ParseResult ProcessedResult { get; }

            public ParseRuleResult(PatternMatchResult patternMatchResult, ParseResult processedResult)
            {
                PatternMatchResult = patternMatchResult;
                ProcessedResult = processedResult;
            }
        }

        private ParseRule GetRuleByKey(string ruleKey)
        {
            if (!Rules.ContainsKey(ruleKey))
                throw new InvalidOperationException($"ParseRule key not found: '{ruleKey}'");
            return Rules[ruleKey];
        }

        public ParseRuleResult ParseRule(string ruleKey, ParsePosition parsePosition, ParseErrorCollector errorCollector)
        {            
            var rule = GetRuleByKey(ruleKey);
            var res = rule.Pattern.Parse(parsePosition, errorCollector);

            if (!res.Success)
                return new ParseRuleResult(res, ParseResult.Error("Error!", parsePosition));

            return new ParseRuleResult(res, rule.BuildResult(res));
        }

        public void AddRule(ParseRule rule)
        {
            if (Rules.ContainsKey(rule.Key))
                throw new InvalidOperationException($"Duplicate ParseRule key '{rule.Key}'");
            Rules.Add(rule.Key, rule);
        }      

        public void AddRule<T>(string ruleKey, IPattern pattern, Func<PatternMatchResult, T> buildResult)
            => AddRule(new ParseRule(ruleKey, pattern, _ => new ParseResult<T>(buildResult(_))));


        public ParseResult Parse(List<Token> tokens)
        {
            tokens.ForEach(_ => Debug.WriteLine($"{_}->{_.ParsePosition}"));
            var errorCollector = new ParseErrorCollector();

            var result = ParseRule(MainRuleKey, ParsePosition.BeginParse(tokens), errorCollector);

            if(!result.PatternMatchResult.Success || !result.PatternMatchResult.ParsePosition.IsOutOfRange)
            {
                if (result.PatternMatchResult.Success) // premature ending
                {
                    errorCollector.Add(PatternMatchResult.Failed(result.PatternMatchResult.ParsePosition)
                        .SetErrorMessage("Input longer than expected"));
                }

                var errorPosition = result.PatternMatchResult.ParsePosition;
                var mre = errorCollector.GetMostRelevantError(errorPosition);
                return ParseResult.Error(mre.ErrorMessage, mre.ParsePosition);
            }

            return result.ProcessedResult;           
        }


    }
}
