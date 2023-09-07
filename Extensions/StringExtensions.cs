using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsotericDevZone.Celesta.Extensions
{
    public static class StringExtensions
    {
        public static string Indent(this string str, string lineDelimiter="\r\n", int spacesCount=4)
        {
            return str
                .Split(new string[] { lineDelimiter }, StringSplitOptions.None)
                .Select(_ => new string(' ', spacesCount) + _)
                .JoinToString(lineDelimiter);
        }

        public static (int Line, int Column) GetLineAndColumn(this string str, int position)
        {
            int lineNumber = str.ToCharArray(0, position).Count(chr => chr == '\n') + 1;
            int fis = str.LastIndexOf("\n", position);
            int posi = position - fis;
            return (lineNumber, posi);
        }

        /// <summary>
        /// Replaces multiple adjacent whitespaces with a single space (' ') character.
        /// E.g. "my  unevenly\t spaced string".RemoveRedundantWhitespace() --> "my unevenly spaced string"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>       
        public static IEnumerable<Match> RemoveRedundantWhitespace(this string input)
        {
            return new Regex(@"[^\s]+")
                .Matches(input)
                .Cast<Match>();
        }

        internal static IEnumerable<Token> ToTokens(this string[] splittedValues, string originalInput, int tokenStartIndex)
        {
            int offset = 0;
            foreach (var value in splittedValues)
            {
                yield return new Token(value, tokenStartIndex + offset, originalInput.GetLineAndColumn(tokenStartIndex + offset));
                offset += value.Length;
            }
            yield break;
        }
    }
}
