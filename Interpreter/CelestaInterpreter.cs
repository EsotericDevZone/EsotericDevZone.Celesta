using EsotericDevZone.Celesta.AST.Utils;
using EsotericDevZone.Celesta.AST;
using EsotericDevZone.Celesta.Definitions.Functions;
using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Celesta.Providers;
using EsotericDevZone.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsotericDevZone.Celesta.Parser;
using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.RuleBasedParser;
using EsotericDevZone.Celesta.Interpreter.Implementations;
using EsotericDevZone.Core;
using System.Diagnostics;

namespace EsotericDevZone.Celesta.Interpreter
{
    public class CelestaInterpreter
    {
        private DataTypeProvider DataTypeProvider = new DataTypeProvider();
        private VariableProvider VariableProvider = new VariableProvider();
        private FunctionProvider FunctionProvider = new FunctionProvider();
        private OperatorProvider OperatorProvider = new OperatorProvider();

        private ASTBuilder ASTBuilder;        

        public DataType IntegerType { get; }
        public DataType DecimalType { get; }
        public DataType StringType { get; }
        public DataType VoidType { get; }
        public DataType BoolType { get; }

        public IImportResolver ImportResolver = new ImportResolver();

        public CelestaInterpreter()
        {
            var Int = IntegerType = DataType.Primitive("int", "", "@main");
            var Decimal = DecimalType = DataType.Primitive("decimal", "", "@main");
            var String = StringType = DataType.Primitive("string", "", "@main");
            var Void = VoidType = DataType.Primitive("void", "", "@main");
            var Bool = BoolType = DataType.Primitive("bool", "", "@main");            
            
            DataTypeProvider.Add(Int);
            DataTypeProvider.Add(Decimal);
            DataTypeProvider.Add(String);
            DataTypeProvider.Add(Void);
            DataTypeProvider.Add(Bool);            

            AddBuiltInFunction("str", Arrays.Of("int"), "string", args => new ValueObject(String, args[0].Value.ToString()));

            AddOperator("+", "int", "int", "int", (a, b) => new ValueObject(Int, (int)a.Value + (int)b.Value));
            AddOperator("-", "int", "int", "int", (a, b) => new ValueObject(Int, (int)a.Value - (int)b.Value));
            AddOperator("*", "int", "int", "int", (a, b) => new ValueObject(Int, (int)a.Value * (int)b.Value));
            AddOperator("/", "int", "int", "int", (a, b) => new ValueObject(Int, (int)a.Value / (int)b.Value));
            AddOperator("%", "int", "int", "int", (a, b) => new ValueObject(Int, (int)a.Value % (int)b.Value));

            AddOperator("==", "int", "int", "bool", (a, b) => new ValueObject(Bool, (int)a.Value == (int)b.Value));
            AddOperator("!=", "int", "int", "bool", (a, b) => new ValueObject(Bool, (int)a.Value != (int)b.Value));
            AddOperator("<", "int", "int", "bool", (a, b) => new ValueObject(Bool, (int)a.Value < (int)b.Value));
            AddOperator("<=", "int", "int", "bool", (a, b) => new ValueObject(Bool, (int)a.Value <= (int)b.Value));
            AddOperator(">", "int", "int", "bool", (a, b) => new ValueObject(Bool, (int)a.Value > (int)b.Value));
            AddOperator(">=", "int", "int", "bool", (a, b) => new ValueObject(Bool, (int)a.Value >= (int)b.Value));

            AddOperator("+", "int", "int", (a) => new ValueObject(Int, (int)a.Value));
            AddOperator("-", "int", "int", (a) => new ValueObject(Int, -(int)a.Value));

            AddOperator("+", "string", "string", "string", (a, b) => new ValueObject(String, (string)a.Value + (string)b.Value));

            var typeDefaults = new DataTypeDefaultValueGetter();
            typeDefaults.SetDefaultValue(Int, () => new IntegerConstantNode(null, 0, Int));
            typeDefaults.SetDefaultValue(Decimal, () => new RealConstantNode(null, 0.0, Decimal));
            typeDefaults.SetDefaultValue(String, () => new StringConstantNode(null, "", String));
            typeDefaults.SetDefaultValue(Bool, () => new BooleanConstantNode(null, false, Bool));

            ASTBuilder = new ASTBuilder(DataTypeProvider, VariableProvider, FunctionProvider, OperatorProvider, typeDefaults);
            ASTBuilder.ImportResolver = ImportResolver;
            ASTBuilder.IntegerConstantType = DataTypeProvider.Find("int", "@main").First();
            ASTBuilder.RealConstantType = DataTypeProvider.Find("decimal", "@main").First();
            ASTBuilder.StringConstantType = DataTypeProvider.Find("string", "@main").First();
            ASTBuilder.VoidType = DataTypeProvider.Find("void", "@main").First();
            ASTBuilder.BooleanConstantType = DataTypeProvider.Find("bool", "@main").First();
        }

        private Dictionary<Operator, OperatorImplementation> OperatorImplementations = new Dictionary<Operator, OperatorImplementation>();
        private Dictionary<Function, FunctionImplementation> FunctionImplementations = new Dictionary<Function, FunctionImplementation>();

        public DataType GetTypeByFullName(string name)
        {
            var identifier = name.ToIdentifier();
            if (identifier == null)
                throw new ArgumentException("Input is not a valid identifier name");
            return DataTypeProvider.Resolve(identifier, "@main", true);
        }

        public void AddBuiltInFunction(string funName, string[] argTypes, string outType, Func<ValueObject[], ValueObject> func)
        {
            var inT = argTypes.Select(GetTypeByFullName).ToArray();
            var outT = GetTypeByFullName(outType);
            var idt = funName.ToIdentifier();
            if(FunctionProvider.Resolve(idt, "@main",inT, false)!=null)
            {
                throw new ArgumentException($"Function already exists: {funName}({inT.JoinToString(",")})");
            }
            var fun = new BuiltInFunction(idt.Name, idt.PackageName, inT, outT);
            FunctionProvider.Add(fun);
            var impl = new FunctionImplementation(fun, func);
            FunctionImplementations[fun] = impl;
        }        

        public void AddOperator(string opName, string in1Type, string in2Type, string outType, Func<ValueObject, ValueObject, ValueObject> func)
        {
            var in1T = GetTypeByFullName(in1Type);
            var in2T = GetTypeByFullName(in2Type);
            var outT = GetTypeByFullName(outType);
            if (OperatorProvider.FindBinary(opName, in1T, in2T).Count() > 0)
                throw new ArgumentException($"Operator already exists: {opName}({in1T},{in2T})");
            var op = Operator.BinaryOperator(opName, in1T, in2T, outT);
            var impl = new OperatorImplementation(op, args => func(args[0], args[1]));
            OperatorProvider.Add(op);
            OperatorImplementations[op] = impl;
        }

        public void AddOperator(string opName, string in1Type, string outType, Func<ValueObject, ValueObject> func)
        {
            var in1T = GetTypeByFullName(in1Type);            
            var outT = GetTypeByFullName(outType);
            if (OperatorProvider.FindUnary(opName, in1T).Count() > 0) 
                throw new ArgumentException($"Operator already exists: {opName}({in1T})");
            var op = Operator.UnaryOperator(opName, in1T, outT);
            var impl = new OperatorImplementation(op, args => func(args[0]));
            OperatorProvider.Add(op);
            OperatorImplementations[op] = impl;
        }

        CelestaParser Parser = new CelestaParser();

        Dictionary<string, ValueObject> Variables = new Dictionary<string, ValueObject>();

        private void SetVariableValue(VariableNode variable, ValueObject value)
        {
            Variables[variable.Variable.FullName] = value;
        }

        private ValueObject GetVariableValue(VariableNode variable)
        {
            return Variables[variable.Variable.FullName];
        }

        private ValueObject Execute(IASTNode node, LocalContext localContext)
        {
            if(node is IntegerConstantNode constInt)
            {
                return new ValueObject(IntegerType, constInt.Value);
            }
            if(node is RealConstantNode constReal)
            {
                return new ValueObject(DecimalType, constReal.Value);
            }
            if(node is StringConstantNode constString)
            {
                return new ValueObject(StringType, constString.Value);
            }            
            if(node is BooleanConstantNode constBool)
            {
                return new ValueObject(BoolType, constBool.Value);
            }

            if(node is IBlockNode blockNode)
            {
                foreach (var child in blockNode.GetChildren())
                {
                    var eval = Execute(child, localContext);
                    if (eval.ScopeMustReturn)
                        return eval;                    
                }
                return new ValueObject(VoidType, null);
            }
            if (node is VariableDeclarationNode vdecl) 
            {
                var variable = vdecl.VariableNode;
                var expression = vdecl.AssignedExpression;
                var value = Execute(expression, localContext);
                SetVariableValue(variable, value);
                return value;
            }
            if(node is VariableNode varNode)
            {
                return GetVariableValue(varNode);
            }
            if(node is OperatorNode op)
            {
                var oper = op.Operator;
                try
                {                    
                    var impl = OperatorImplementations[oper];
                    ValueObject result = impl.Operation(op.Arguments.Select(_ => Execute(_, localContext)).ToArray());
                    if (result.DataType.FullName != oper.OutputType.FullName)
                        throw new InvalidOperationException("Operator returned a different type than expected");
                    return result;
                }
                catch(KeyNotFoundException)
                {
                    throw new ArgumentException($"No implementation found for operator {oper}");
                }
            }
            if(node is FunctionFormalParameterNode formalParam)
            {
                if (localContext == null)
                    throw new ArgumentException("DEBUG FATAL : Function local context expected");
                return localContext.FormalParameters[formalParam.Name];
            }
            if(node is FunctionDeclarationNode fdecl)
            {
                var fun = fdecl.Function;                
                if (FunctionImplementations.ContainsKey(fun))
                    throw new ArgumentException($"Function implementation already defined : {fun}");

                FunctionImplementations[fun] = new FunctionImplementation(fun, args =>
                {
                    var context = new LocalContext(localContext);
                    var udf = (fdecl.Function as UserDefinedFunction);
                    for (int i=0;i<udf.FormalParameters.Length;i++)
                    {
                        context.FormalParameters[udf.FormalParameters[i].Name] = args[i];
                    }
                    var result = Execute(fdecl.Body, context);
                    result.ScopeMustReturn = false;
                    return result;
                });


                return new ValueObject(VoidType, null);
            }
            if (node is FunctionCallNode funcall) 
            {
                try
                {
                    var f = FunctionImplementations[funcall.Function];
                    var result = f.Operation(funcall.Arguments.Select(_=>Execute(_,localContext)).ToArray());
                    if (result.DataType.FullName != funcall.OutputType.FullName) 
                        throw new InvalidOperationException("Function returned a different type than expected");
                    return result;
                }
                catch(KeyNotFoundException)
                {
                    throw new ArgumentException($"No implementation found for function : {funcall.Function}");
                }
            }            
            if(node is ReturnNode ret)
            {
                if (ret.OutputType.FullName == VoidType.FullName)
                    return new ValueObject(VoidType, null) { ScopeMustReturn = true };
                var result = Execute(ret.ReturnedExpression, localContext);
                result.ScopeMustReturn = true;
                return result;
            }
            if(node is IfNode ifNode)
            {
                var condition = Execute(ifNode.Condition, localContext);
                if((bool)condition.Value)
                {
                    var value = Execute(ifNode.ThenBranch, localContext);
                    if (value.ScopeMustReturn)
                        return value;
                }
                else if(ifNode.ElseBranch!=null)
                {
                    var value = Execute(ifNode.ElseBranch, localContext);
                    if (value.ScopeMustReturn)
                        return value;
                }
                return new ValueObject(VoidType, null);
            }
            if(node is WhileNode whileNode)
            {
                while((bool)Execute(whileNode.Condition, localContext).Value)
                {
                    var value = Execute(whileNode.LoopLogic, localContext);
                    if (value.ScopeMustReturn)
                        return value;
                }
                return new ValueObject(VoidType, null);
            }
            if(node is AssignmentNode assign)
            {
                var lhs = assign.LeftHandSide;
                var rhs = assign.RightHandSide;

                if (lhs is VariableNode variable)
                {
                    var value = Execute(rhs, localContext);
                    SetVariableValue(variable, value);
                    return new ValueObject(VoidType, null);
                }
                else if (lhs is FunctionFormalParameterNode fParam) 
                {
                    var value = Execute(rhs, localContext);
                    localContext.FormalParameters[fParam.Name] = value;
                    return new ValueObject(VoidType, null);
                }
                else
                {
                    throw new NotImplementedException("Invalid left-hand-side in assignment");
                }
            }
            if(node is RepeatNNode reptN)
            {
                int count = (int)Execute(reptN.Count, localContext).Value;
                for(int i=0;i<count;i++)
                {
                    var value = Execute(reptN.LoopLogic, localContext);
                    if (value.ScopeMustReturn)
                        return value;
                }
                return new ValueObject(VoidType, null);
            }
            if (node is TypeAliasNode) 
            {
                return new ValueObject(VoidType, null);
            }            
            if(node is ImportNode importNode)
            {
                if(ImportedSources.Contains(importNode.Source))
                    return new ValueObject(VoidType, null);

                var toImportQ = ASTBuilder.Imports.Where(_ => _.Source == importNode.Source).ToArray();
                var toImport = toImportQ.Length == 0 ? null : toImportQ[0].Node;                

                Execute(toImport, localContext);


                return new ValueObject(VoidType, null);
            }

            throw new ArgumentException($"Node not implemented : {node.GetType()}");            
        }

        private ISet<string> ImportedSources = new HashSet<string>();

        public object Execute(string input)
        {
            var _node = Parser.Parse<IParseTreeNode>(input);
            var _ast = ASTBuilder.BuildNode(_node);
            Console.WriteLine(_ast);

            AbstractASTNode.AssertAllNodes<FunctionDeclarationNode>(_ast, nd => !(nd.Function is SyscallFunction),
                nd => throw new ArgumentException("Syscall functions are not supported by this interpreter"));
            //_ast.
            return Execute(_ast, null).Value;
        }

        public static CelestaInterpreter ConsoleDefault
        {
            get
            {
                var interpreter = new CelestaInterpreter();

                interpreter.AddOperator("+", "string", "int", "string", (str, x) =>
                {
                    return new ValueObject(interpreter.GetTypeByFullName("string"),
                        (str.Value as string) + ((int)x.Value).ToString());
                });

                interpreter.AddBuiltInFunction("println", Arrays.Of("string"), "void", args =>
                {
                    Console.WriteLine(args[0].Value);
                    return new ValueObject(interpreter.VoidType, null);
                });

                interpreter.AddBuiltInFunction("print", Arrays.Of("string"), "void", args =>
                {
                    Console.Write(args[0].Value);
                    return new ValueObject(interpreter.VoidType, null);
                });

                interpreter.AddBuiltInFunction("readline", Arrays.Empty<string>(), "string", args =>
                {
                    return new ValueObject(interpreter.StringType, Console.ReadLine());
                });

                interpreter.AddBuiltInFunction("readint", Arrays.Empty<string>(), "int", args =>
                {
                    return new ValueObject(interpreter.IntegerType, Convert.ToInt32(Console.ReadLine()));
                });

                return interpreter; 
            }
        }
    }
}
