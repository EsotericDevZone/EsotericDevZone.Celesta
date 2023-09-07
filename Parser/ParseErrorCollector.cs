using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Parser.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Parser
{
    public class ParseErrorCollector
    {
        public List<PatternMatchResult> Errors { get; } = new List<PatternMatchResult>();

        public PatternMatchResult Add(PatternMatchResult matchResult)
        {
            Errors.Add(matchResult);
            return matchResult;
        }

        class PatternMatchResultEqualityComparer : IEqualityComparer<PatternMatchResult>
        {
            private string GetKey(PatternMatchResult pmr) => $"{pmr.ParsePosition.Index}:{pmr.ErrorMessage}";

            public bool Equals(PatternMatchResult x, PatternMatchResult y)
            {
                return GetKey(x) == GetKey(y);
            }

            public int GetHashCode(PatternMatchResult obj)
            {
                return GetKey(obj).GetHashCode();
            }
        }

        public PatternMatchResult GetMostRelevantError(ParsePosition errorPosition)
        {
            var errors = Errors.Select(DispatchResults).SelectMany(_ => _)
                .Where(_ => errorPosition.Index <= _.ParsePosition.Index && !string.IsNullOrEmpty(_.ErrorMessage))
                .OrderByDescending(_ => _.ParsePosition.Index)
                .OrderByDescending(_ => _.ErrorPriority)
                .Distinct(new PatternMatchResultEqualityComparer())
                .ToList();

            return PatternMatchResult.Failed(errorPosition)
                .SetErrorMessage(errors.Select(_ => $"{_.ParsePosition.GetLineColumn()}: {_.ErrorMessage}").JoinToString(Environment.NewLine));

            //return errors.FirstOrDefault() ?? PatternMatchResult.Failed(errorPosition).SetErrorMessage("Unknown error");
        }

        private IEnumerable<PatternMatchResult> DispatchResults(PatternMatchResult result)
        {
            while(result!=null)
            {
                yield return result;
                result = result.InnerResult;
            }
            yield break;
        }
    }
}
