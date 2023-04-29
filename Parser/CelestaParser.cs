using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.RuleBasedParser;
using EsotericDevZone.RuleBasedParser.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Parser
{
    public class CelestaParser : RuleBasedParser.Parser
    {
        public CelestaParser() : base(new CelestaTokensSplitOptions(), CommentStyles.CStyle)
        {
            RegisterAtom("SYMBOL", ParseTreeAtomBuilders.SymbolLiteral);
            RegisterAtom("NUMBER", ParseTreeAtomBuilders.NumberLiteral);
            RegisterAtom("INT", AtomBuilders.Integer);
            RegisterAtom("STRING", ParseTreeAtomBuilders.StringLiteral);

            ParseRules.RegisterRule("@IDENTIFIER", "SYMBOL ?? # SYMBOL", ParseTreeNodeBuilders.Identifier);                        

            var operatorsRuleBuilder = new OperatorsRuleBuilder();
            operatorsRuleBuilder.AddLayer("or");
            operatorsRuleBuilder.AddLayer("and");
            operatorsRuleBuilder.AddLayer("==", "!=");
            operatorsRuleBuilder.AddLayer("<", "<=", ">", ">=");
            operatorsRuleBuilder.AddLayer("+", "-");
            operatorsRuleBuilder.AddLayer("*", "/", "%");
            operatorsRuleBuilder.GetRules("@EXPR", "@TERM").ForEach(r =>
                ParseRules.RegisterRule(r.Key, r.Pattern, r.Builder));


            ParseRules.RegisterRule("@EXPRLIST", "@EXPR ?? , @EXPR", ParseTreeNodeBuilders.__List);            
            
            ParseRules.RegisterRule("@STERM", "@IDENTIFIER", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@STERM", "NUMBER", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@STERM", "STRING", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@STERM", "( @EXPR )", ParseResultBuilders.Self);                        

            ParseRules.RegisterRule("@FTERM", "@STERM . SYMBOL ?? . SYMBOL", ParseTreeNodeBuilders.NameAccessor);
            ParseRules.RegisterRule("@FTERM", "@STERM", ParseResultBuilders.Self);


            ParseRules.RegisterRule("@FUNCALL", "@FTERM ( )", ParseTreeNodeBuilders.FunctionCall);
            ParseRules.RegisterRule("@FUNCALL", "@FTERM ( @EXPRLIST )", ParseTreeNodeBuilders.FunctionCall);            

            ParseRules.RegisterRule("@TERM", "@FUNCALL", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@TERM", "@FTERM", ParseResultBuilders.Self);

            ParseRules.RegisterRule("@TERM", "@FTERM", ParseResultBuilders.Self);

            ParseRules.RegisterRule("@ASSIGN", "@TERM = @EXPR", ParseTreeNodeBuilders.Assignment);

            ParseRules.RegisterRule("@IF", "if @EXPR then @BLOCK else @BLOCK end|endif", ParseTreeNodeBuilders.If);
            ParseRules.RegisterRule("@IF", "if @EXPR then @BLOCK end|endif", ParseTreeNodeBuilders.If);

            ParseRules.RegisterRule("@WHILE", "while @EXPR do @BLOCK end|endwhile", ParseTreeNodeBuilders.While);
            ParseRules.RegisterRule("@REPTN", "repeat @EXPR do @BLOCK end|endrepeat", ParseTreeNodeBuilders.RepeatN);

            ParseRules.RegisterRule("@VDECL", "@IDENTIFIER SYMBOL = @EXPR", ParseTreeNodeBuilders.VariableDeclaration);
            ParseRules.RegisterRule("@VDECL", "@IDENTIFIER SYMBOL", ParseTreeNodeBuilders.VariableDeclaration);

            ParseRules.RegisterRule("@TALIAS", "type SYMBOL = @IDENTIFIER", ParseTreeNodeBuilders.TypeAlias);
            ParseRules.RegisterRule("@TALIAS", "type SYMBOL =! @IDENTIFIER", ParseTreeNodeBuilders.TypeAliasIsolated);

            ParseRules.RegisterRule("@ARGDECL", "@IDENTIFIER SYMBOL", ParseTreeNodeBuilders.ArgumentDeclaration);

            ParseRules.RegisterRule("@FDECLARGS", "@ARGDECL ?? , @ARGDECL", ParseTreeNodeBuilders.__List);

            ParseRules.RegisterRule("@SYSCALLFDECL", "syscall INT function SYMBOL ( ) : @IDENTIFIER", ParseTreeNodeBuilders.SyscallFunctionHeader);
            ParseRules.RegisterRule("@SYSCALLFDECL", "syscall INT function SYMBOL ( @FDECLARGS ) : @IDENTIFIER", ParseTreeNodeBuilders.SyscallFunctionHeader);

            ParseRules.RegisterRule("@FDECL", "function SYMBOL ( ) : @IDENTIFIER begin @BLOCK end|endfunction", ParseTreeNodeBuilders.FunctionDeclaration);
            ParseRules.RegisterRule("@FDECL", "function SYMBOL ( @FDECLARGS ) : @IDENTIFIER begin @BLOCK end|endfunction", ParseTreeNodeBuilders.FunctionDeclaration);

            ParseRules.RegisterRule("@PACKAGE", "package SYMBOL @BLOCK end|endpackage", ParseTreeNodeBuilders.Package);

            ParseRules.RegisterRule("@BLOCK", "@INSTR ; ?? @INSTR ;", ParseTreeNodeBuilders.Block);

            ParseRules.RegisterRule("@INSTR", "@WHILE", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@INSTR", "@REPTN", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@INSTR", "@IF", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@INSTR", "@PACKAGE", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@INSTR", "@TALIAS", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@INSTR", "@SYSCALLFDECL", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@INSTR", "@FDECL", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@INSTR", "@VDECL", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@INSTR", "@ASSIGN", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@INSTR", "@FUNCALL", ParseResultBuilders.Self);            

            ParseRules.RegisterRule("@CODE", "@BLOCK", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@CODE", "@INSTR", ParseResultBuilders.Self);
            ParseRules.RegisterRule("@CODE", "@EXPR", ParseResultBuilders.Self);



            RootRuleKey = "@CODE";
        }

        
    }
}
