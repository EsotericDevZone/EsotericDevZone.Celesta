using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.RuleBasedParser;
using EsotericDevZone.RuleBasedParser.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EsotericDevZone.Celesta.Parser
{
    internal class OperatorsRuleBuilder
    {
        List<string[]> OperatorLayers = new List<string[]>();
        public void AddLayer(params string[] layer) => OperatorLayers.Add(layer);


        public List<(string Key, string Pattern, Func<ParseResult[], Token[], ParseResult> Builder)> GetRules(string expressionKey, string terminalKey)
        {
            var result = new List<(string Key, string Pattern, Func<ParseResult[], Token[], ParseResult>)>();

            string GetKeyName(int layer)
            {
                if (layer == 0) return expressionKey;
                if (layer == OperatorLayers.Count) return terminalKey;
                return $"{expressionKey}_E{layer}";
            }

            for (int i = 0; i < OperatorLayers.Count; i++)
            {
                string thisKey = GetKeyName(i);
                string nextKey = GetKeyName(i + 1);
                string ops = string.Join("|", OperatorLayers[i]);

                result.Add((thisKey, $"{nextKey} ?? {ops} {nextKey}", ParseResultBuilders.LeftAssociate((r1, r2, tk) =>
                {
                    return new ParseResult(tk,
                        new BinaryOperator(tk, r1.Value as IParseTreeNode, r2.Value as IParseTreeNode));
                })));
            }            

            return result;
        }       
    }
}
