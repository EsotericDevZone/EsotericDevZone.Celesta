using EsotericDevZone.Celesta.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Parser
{
    public abstract class ParseResult
    {
        public string Label { get; set; }
        public object Value { get; }

        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public ParsePosition ErrorPosition { get; private set; }

        public ParseResult(object value)
        {
            Success = true;
            Value = value;
        }

        public T GetValue<T>()
        {
            if (Value is T result)
                return result;
            throw new InvalidCastException($"Expected {typeof(T)}, actual value is {Value?.GetType()?.ToString() ?? "null"}");
        }

        public static ParseResult Error(string message, ParsePosition parsePosition)
        {
            return new ParseResult<object>(null) { Success = false, ErrorMessage = message, ErrorPosition = parsePosition };
        }

        public override string ToString()
        {
            if (Success)
                return $"ParseResult{{{((Value is ParseResult[] pr) ? pr.JoinToString(", ") : Value)}}}";
            else
                return $"ParseResult{{Fail,{ErrorMessage} at {ErrorPosition}}}";
        }
    }

    public interface IParseResult<out T> 
    { 
        T Value { get; }
    }

    public class ParseResult<T> : ParseResult, IParseResult<T>
    {
        public ParseResult(T value) : base(value) { }
        public new T Value => (T)base.Value;
    }
}
