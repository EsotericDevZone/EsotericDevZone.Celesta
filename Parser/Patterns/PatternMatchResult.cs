using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public class PatternMatchResult
    {
        public bool Success { get; }
        public ParsePosition ParsePosition { get; }
        public ParseResult ParseResult { get; }                

        
        public string Label { get; private set; }

        public PatternMatchResult SetLabel(string label)
        {
            Label = label;
            if (ParseResult != null)
                ParseResult.Label = label;            
            return this;
        }

        #region ErrorData
        public PatternMatchResult InnerResult { get; private set; }
        public string ErrorMessage { get; private set; }
        public int ErrorPriority { get; private set; }

        public PatternMatchResult SetInnerResult(PatternMatchResult innerResult)
        {
            InnerResult = innerResult;
            return this;
        }

        public PatternMatchResult SetErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
            return this;
        }

        public PatternMatchResult SetErrorPriority(int value)
        {
            ErrorPriority = value;
            return this;
        }

        #endregion

        public PatternMatchResult(bool success, ParsePosition parsePosition, ParseResult parseResult)
        {
            Success = success;
            ParsePosition = parsePosition;
            ParseResult = parseResult;
        }        

        public static PatternMatchResult Succeeded<T>(ParsePosition parsePosition, T parseResult)
        {
            return new PatternMatchResult(true, parsePosition, new ParseResult<T>(parseResult));
        }

        public static PatternMatchResult Failed(ParsePosition parsePosition)
            => new PatternMatchResult(false, parsePosition, null);

        public override string ToString()
        {
            if (Success)
            {
                return $"PatternMatchResult{{Success, Position={ParsePosition.Index}, Result={ParseResult.Value?.ToString() ?? "<null>"}}}";
            }
            else
                return $"PatternMatchResult{{Fail, Position={ParsePosition.Index}, Message={ErrorMessage ?? "<null>"}, Inner={InnerResult}}}";

        }
    }
}
