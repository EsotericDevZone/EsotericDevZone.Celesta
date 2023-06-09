﻿using EsotericDevZone.Celesta.AST.Utils;
using EsotericDevZone.Celesta.Counters;
using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Celesta.Definitions.Functions;
using EsotericDevZone.Celesta.Parser;
using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.Celesta.Providers;
using EsotericDevZone.Core;
using EsotericDevZone.Core.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EsotericDevZone.Celesta.AST
{
    public class ASTBuilder
    {
        public DataType IntegerConstantType { get; set; }
        public DataType RealConstantType { get; set; }
        public DataType StringConstantType { get; set; }
        public DataType BooleanConstantType { get; set; }
        public DataType VoidType { get; set; }
        public IDataTypeDefaultValueGetter DataTypeDefaultValues { get; internal set; }


        private IDataTypeProvider DataTypeProvider;
        private IVariableProvider VariableProvider;
        private IFunctionProvider FunctionProvider;
        private IOperatorProvider OperatorProvider;

        private IStringCounter ScopeCounter = new FunctionalStringCounter(i => $"@_s{i}");

        public ASTBuilder(IDataTypeProvider dataTypeProvider, IVariableProvider variableProvider
            , IFunctionProvider functionProvider, IOperatorProvider operatorProvider, IDataTypeDefaultValueGetter defaultValues)
        {
            DataTypeProvider = dataTypeProvider;
            VariableProvider = variableProvider;
            FunctionProvider = functionProvider;
            OperatorProvider = operatorProvider;
            DataTypeDefaultValues = defaultValues;
        }

        public IASTNode BuildNode(IParseTreeNode parseTreeNode)
        {
            var tempDataTypeProv = new DataTypeProvider();
            tempDataTypeProv.AddFromProvider(DataTypeProvider);

            var tempVariableProv = new VariableProvider();
            tempVariableProv.AddFromProvider(VariableProvider);

            var tempFunctionProv = new FunctionProvider();
            tempFunctionProv.AddFromProvider(FunctionProvider);

            var tempOperatorProv = new OperatorProvider();
            tempOperatorProv.AddFromProvider(OperatorProvider);

            var buildResult = BuildNodeRec(parseTreeNode, tempDataTypeProv, tempVariableProv
                , tempFunctionProv, tempOperatorProv, null);

            if (buildResult.Failed) 
            {
                throw new ASTBuildNodeException(buildResult.ErrorMessage);
            }

            ValidateAST(buildResult.ASTNode);

            DataTypeProvider.CopyFromProvider(tempDataTypeProv);
            VariableProvider.CopyFromProvider(tempVariableProv);
            FunctionProvider.CopyFromProvider(tempFunctionProv);

            return buildResult.ASTNode;

        }

        private BuildNodeResult BuildNodeRec(IParseTreeNode parseTreeNode
            , IDataTypeProvider dataTypeProv, IVariableProvider variableProv
            , IFunctionProvider functionProv, IOperatorProvider operatorProv,
            IASTNode parent)
        {
            if(parseTreeNode is BoolLiteral boolLiteral)
            {
                return BuildNodeResult.Node(new BooleanConstantNode(parent, boolLiteral.Value == "true", BooleanConstantType));
            }
            if (parseTreeNode is NumericLiteral numericLiteral) 
            {
                var valueStr = numericLiteral.Value;
                if (int.TryParse(valueStr, out var intValue))
                {
                    return BuildNodeResult.Node(new IntegerConstantNode(parent, intValue, IntegerConstantType));
                }
                else if (double.TryParse(valueStr, out var realValue))
                {
                    return BuildNodeResult.Node(new RealConstantNode(parent, realValue, RealConstantType));
                }
                else
                {
                    return BuildNodeResult.Error($"Unable to parse numeric literal : {valueStr}");
                }
            }
            if (parseTreeNode is StringLiteral stringLiteral)
            {
                var strValue = stringLiteral.Value.Substring(1, stringLiteral.Value.Length - 2);
                //Console.WriteLine("Val = " + strValue);
                var node = new StringConstantNode(parent, strValue.FromLiteral(), StringConstantType);
                return BuildNodeResult.Node(node);
            }            
            if (parseTreeNode is VariableDeclaration varDeclaraction)
            {
                var varName = varDeclaraction.VariableName;
                var typeIdentifier = varDeclaraction.DataType;
                var scope = AbstractASTNode.GetScope(parent);

                if (parent?.IsIncludedIn(typeof(FunctionDeclarationNode)) ?? false) 
                {
                    var funDecl = parent.GetClosestParent<FunctionDeclarationNode>();
                    var function = funDecl.Function;
                    if (function is UserDefinedFunction udf)
                    {
                        var formalParams = udf.FormalParameters;

                        var param = udf.FormalParameters.Where(p => p.Name == varName).FirstOrDefault();
                        if (param != null)
                        {
                            return BuildNodeResult.Error($"Declared variable has the same name with formal parameter '{param.Name}'");
                        }
                    }
                }

                var dataType = dataTypeProv.Resolve(typeIdentifier, scope, false);
                if (dataType == null)
                    return BuildNodeResult.Error($"Unidentified data type : {typeIdentifier.FullName}");                    

                var package = AbstractASTNode.GetPackage(parent);

                if (variableProv.Find(package, varName, scope).Count() > 0) 
                {                    
                    return BuildNodeResult.Error($"Duplicate variable definition : {varName}");
                }

                var variable = new Variable(varName, package, scope, dataType, varDeclaraction.IsParameter, varDeclaraction.ParamId);

                var decl = new VariableDeclarationNode(parent);

                var variableNode = new VariableNode(decl, variable);                
                decl.VariableNode = variableNode;

                var assignedExpression = varDeclaraction.InitializationValue != null
                    ? BuildNodeRec(varDeclaraction.InitializationValue, dataTypeProv, variableProv, functionProv, operatorProv, decl)
                    : BuildNodeResult.Node(DataTypeDefaultValues.GetDefaultValue(dataType));

                if (assignedExpression.Failed)
                    return assignedExpression;

                assignedExpression = ExpectExpression(assignedExpression, out var assignExprNode);
                if (assignedExpression.Failed)
                    return assignedExpression;

                decl.AssignedExpression = assignExprNode;                

                if (decl.AssignedExpression.OutputType != variable.DataType) 
                {
                    if (variable.DataType.IsAliasOf(decl.AssignedExpression.OutputType)) 
                    {
                        // ...
                    }
                    else
                    {
                        return BuildNodeResult.Error("Type mismatch in variable declaration");
                    }
                }

                variableProv.Add(variable);                

                return BuildNodeResult.Node(decl);
            }
            if(parseTreeNode is Block block)
            {
                var node = new ScopedNode(parent, ScopeCounter);
                foreach(var child in block.Children)
                {
                    var cresult = BuildNodeRec(child, dataTypeProv, variableProv, functionProv, operatorProv, node);
                    if (cresult.Failed)
                        return cresult;
                    node.AddChild(cresult.ASTNode);
                }
                return BuildNodeResult.Node(node);
            }
            if(parseTreeNode is Identifier identifier)
            {                
                if (identifier.PackageName == "")
                {
                    if (parent?.IsIncludedIn(typeof(FunctionDeclarationNode)) ?? false) 
                    {
                        var funDecl = parent.GetClosestParent<FunctionDeclarationNode>();
                        var function = funDecl.Function;                        
                        if (function is UserDefinedFunction udf) 
                        {                            
                            var formalParams = udf.FormalParameters;

                            var param = udf.FormalParameters.Where(p => p.Name == identifier.Name).FirstOrDefault();
                            if(param!=null)
                            {                                
                                return BuildNodeResult.Node(new FunctionFormalParameterNode(parent, param));
                            }
                        }
                    }
                }                

                var scope = AbstractASTNode.GetScope(parent);                
                var variable = variableProv.Resolve(identifier, scope, false);
                if(variable==null)
                {
                    return BuildNodeResult.Error($"Unknown variable : {identifier.FullName}");
                }               
                var variableNode = new VariableNode(parent, variable);
                return BuildNodeResult.Node(variableNode);
            }
            if (parseTreeNode is Assignment assignment) 
            {
                var assign = new AssignmentNode(parent);

                var lhs = BuildNodeRec(assignment.LeftHandSide, dataTypeProv, variableProv, functionProv, operatorProv, assign);
                if (lhs.Failed)
                    return lhs;
                if (!(lhs.ASTNode is IExpressionNode lhsexpr))
                    return BuildNodeResult.Error("Left-hand side must be a typed expression");
                assign.LeftHandSide = lhsexpr;

                var rhs = BuildNodeRec(assignment.RightHandSide, dataTypeProv, variableProv, functionProv, operatorProv, assign);
                if (rhs.Failed)
                    return rhs;
                if (!(rhs.ASTNode is IExpressionNode rhsexpr))
                    return BuildNodeResult.Error("Right-hand side must be a typed expression");
                assign.RightHandSide = rhsexpr;
                return BuildNodeResult.Node(assign);
            }
            if (parseTreeNode is BinaryOperator binOp)
            {
                var opNode = new OperatorNode(parent);
                var left = BuildNodeRec(binOp.FirstMember, dataTypeProv, variableProv, functionProv, operatorProv, opNode);
                left = ExpectExpression(left, out var leftExpr);
                if (left.Failed)
                    return left;

                var right = BuildNodeRec(binOp.SecondMember, dataTypeProv, variableProv, functionProv, operatorProv, opNode);
                right = ExpectExpression(right, out var rightExpr);
                if (right.Failed)
                    return right;

                var op = operatorProv.FindBinary(binOp.OperatorToken.Value, leftExpr.OutputType, rightExpr.OutputType)
                    .FirstOrDefault();

                if (op == null)
                    return BuildNodeResult.Error($"Operator not found :  {binOp.OperatorToken.Value}({leftExpr.OutputType},{rightExpr.OutputType})");

                opNode.Operator = op;
                opNode.Arguments = Arrays.Of(leftExpr, rightExpr);
                return BuildNodeResult.Node(opNode);
            }
            if (parseTreeNode is UnaryOperator unOp)
            {
                var opNode = new OperatorNode(parent);
                var member = BuildNodeRec(unOp.Expression, dataTypeProv, variableProv, functionProv, operatorProv, opNode);
                member = ExpectExpression(member, out var memberExpr);
                if (member.Failed)
                    return member;
                

                var op = operatorProv.FindUnary(unOp.OperatorToken.Value, memberExpr.OutputType)
                    .FirstOrDefault();

                if (op == null)
                    return BuildNodeResult.Error($"Operator not found :  {unOp.OperatorToken.Value}({memberExpr.OutputType})");

                opNode.Operator = op;
                opNode.Arguments = Arrays.Of(memberExpr);
                return BuildNodeResult.Node(opNode);
            }
            if (parseTreeNode is Package pack) 
            {
                var packNode = new PackageNode(parent, pack.Name);

                foreach (var child in pack.Content.Children) 
                {
                    var cresult = BuildNodeRec(child, dataTypeProv, variableProv, functionProv, operatorProv, packNode);
                    if (cresult.Failed)
                        return cresult;
                    packNode.AddChild(cresult.ASTNode);
                }
                return BuildNodeResult.Node(packNode);
            }
            if(parseTreeNode is IfBlock ifBlock)
            {
                var ifNode = new IfNode(parent);

                var condition = BuildNodeRec(ifBlock.Condition, dataTypeProv, variableProv, functionProv, operatorProv, ifNode);
                condition = ExpectExpression(condition, out var condExpr);
                if (condition.Failed)
                    return condition;

                var thenBranch = BuildNodeRec(ifBlock.ThenBranch, dataTypeProv, variableProv, functionProv, operatorProv, ifNode);

                if (thenBranch.Failed) return thenBranch;

                var elseBranch = ifBlock.ElseBranch != null
                    ? BuildNodeRec(ifBlock.ElseBranch, dataTypeProv, variableProv, functionProv, operatorProv, ifNode)
                    : null;

                if (elseBranch?.Failed ?? false) return elseBranch;

                ifNode.Condition = condExpr;
                ifNode.ThenBranch = thenBranch.ASTNode;
                ifNode.ElseBranch = elseBranch?.ASTNode;

                return BuildNodeResult.Node(ifNode);
            }
            if(parseTreeNode is WhileBlock whileBlock)
            {
                var whileNode = new WhileNode(parent);

                var condition = BuildNodeRec(whileBlock.Condition, dataTypeProv, variableProv, functionProv, operatorProv, whileNode);
                condition = ExpectExpression(condition, out var condExpr);
                if (condition.Failed)
                    return condition;

                var loopLogic = BuildNodeRec(whileBlock.Loop, dataTypeProv, variableProv, functionProv, operatorProv, whileNode);
                if (loopLogic.Failed) return loopLogic;

                whileNode.Condition = condExpr;
                whileNode.LoopLogic = loopLogic.ASTNode;
                return BuildNodeResult.Node(whileNode);
            }
            if(parseTreeNode is RepeatNBlock reptN)
            {
                var reptNode = new RepeatNNode(parent);

                var count = BuildNodeRec(reptN.Count, dataTypeProv, variableProv, functionProv, operatorProv, reptNode);
                count = ExpectExpression(count, out var countExpr);
                if (count.Failed)
                    return count;

                var loopLogic = BuildNodeRec(reptN.Loop, dataTypeProv, variableProv, functionProv, operatorProv, reptNode);
                if (loopLogic.Failed) return loopLogic;

                reptNode.Count = countExpr;
                reptNode.LoopLogic = loopLogic.ASTNode;
                return BuildNodeResult.Node(reptNode);
            }
            if (parseTreeNode is FunctionDeclaration fdecl) 
            {                
                var package = AbstractASTNode.GetPackage(parent);
                var scope = AbstractASTNode.GetScope(parent);

                var declArgs = fdecl.Arguments;
                var declOut = fdecl.DataType;

                List<DataType> argTypes = new List<DataType>();

                foreach(var arg in declArgs)
                {
                    var dt = dataTypeProv.Resolve(arg.DataType, scope, false);
                    if(dt==null)
                        return BuildNodeResult.Error($"Unknown type : {arg.DataType}");                    
                    argTypes.Add(dt);
                }


                var outDataType = dataTypeProv.Resolve(declOut, scope, false);

                if (outDataType == null)
                    return BuildNodeResult.Error($"Unknown type : {outDataType}");

                var udf = new UserDefinedFunction(fdecl.Name, package, scope, argTypes.ToArray(), outDataType);
                udf.FormalParameters = declArgs
                    .Select((d, i) => (d, i))
                    .Zip(argTypes, (di, t) => new FunctionFormalParameter(di.d.Name, t, di.i))
                    .ToArray();
                if (udf.FormalParameters.Length != udf.FormalParameters.Select(_ => _.Name).Distinct().Count())
                    return BuildNodeResult.Error("Formal parameters with duplicate names are not allowed");

                var udfNode = new FunctionDeclarationNode(parent) { Function = udf };                

                var body = BuildNodeRec(fdecl.Body, dataTypeProv, variableProv, functionProv, operatorProv, udfNode);
                if (body.Failed) return body;
                
                udfNode.Body = body.ASTNode;
                udf.Body = body.ASTNode;

                functionProv.Add(udf);
                return BuildNodeResult.Node(udfNode);
            }
            if(parseTreeNode is Return ret)
            {
                var retNode = new ReturnNode(parent);
                if (ret.Expression == null)
                {                    
                    retNode.ReturnedExpression = new VoidExpressionNode(retNode, VoidType);
                    return BuildNodeResult.Node(retNode);
                }
                else
                {                    
                    var exprNode = BuildNodeRec(ret.Expression, dataTypeProv, variableProv, functionProv, operatorProv, retNode);                    
                    exprNode = ExpectExpression(exprNode, out var exprNodeExpr);
                    if (exprNode.Failed) return exprNode;
                    retNode.ReturnedExpression = exprNodeExpr;
                    return BuildNodeResult.Node(retNode);
                }
            }
            if(parseTreeNode is FunctionCall funcall)
            {
                var funcallNode = new FunctionCallNode(parent);
                var functor = funcall.Function;
                var args = funcall.Arguments;
                var scope = AbstractASTNode.GetScope(parent);

                var argNodes = new List<IExpressionNode>();
                foreach(var arg in args)
                {
                    var argNode = BuildNodeRec(arg, dataTypeProv, variableProv, functionProv, operatorProv, funcallNode);
                    argNode = ExpectExpression(argNode, out var argNodeExpr);
                    if (argNode.Failed) return argNode;
                    argNodes.Add(argNodeExpr);
                }

                var argTypes = argNodes.Select(_ => _.OutputType).ToArray();

                Function function = null;
                if (functor is Identifier funIdentifier) 
                {
                    //function = functionProv.Resolve(funIdentifier, scope, argTypes, strict: true);
                    function = functionProv.Fit(funIdentifier, scope, argTypes, strict: true);
                    if (function == null)
                        return BuildNodeResult.Error($"No function defined: {funIdentifier.FullName}({argTypes.JoinToString(",")})");                    
                }
                else return BuildNodeResult.Error("Non-identifier functions are work in progress...");

                if (function == null) 
                {
                    return BuildNodeResult.Error($"No function named: {funIdentifier.FullName}");
                }

                funcallNode.Function = function;
                funcallNode.Arguments = argNodes.ToArray();
                return BuildNodeResult.Node(funcallNode);
            }
            if(parseTreeNode is TypeAlias typeAlias)
            {
                var name = typeAlias.TypeName;
                var package = AbstractASTNode.GetPackage(parent);
                var scope = AbstractASTNode.GetScope(parent);
                var refType = dataTypeProv.Resolve(typeAlias.ReferencedType, scope, false);
                if (refType == null)
                    return BuildNodeResult.Error($"Type not found : {typeAlias.ReferencedType}");

                if (dataTypeProv.Resolve(new Identifier(package, name), scope, false) != null)
                    return BuildNodeResult.Error($"Target type already exists : {package}#{name}");

                bool isolated = typeAlias.Isolated;
                var newDataType = DataType.Alias(name, package, scope, refType, isolated);

                var aliasNode = new TypeAliasNode(parent, newDataType, refType);

                dataTypeProv.Add(newDataType);
                return BuildNodeResult.Node(aliasNode);
            }
            if(parseTreeNode is Import import)
            {
                string path = "";

                try
                {
                    path = ImportResolver?.GetSourcePath(import.Source, import.IsFilePath);
                }
                catch (FileNotFoundException e)
                {
                    return BuildNodeResult.Error(e.Message);
                }

                if (path == null)
                    return BuildNodeResult.Error($"Unable to import dependency : '{import.Source}'");
                path = Path.GetFullPath(path);
                if (!File.Exists(path))
                    return BuildNodeResult.Error($"Import file not found : '{path}'");

                if (!Imports.Any(_ => _.Source == path)) 
                {
                    var source = File.ReadAllText(path);

                    var importParseTreeNode = new CelestaParser().Parse<IParseTreeNode>(source);
                    var importNodeR = BuildNodeRec(importParseTreeNode, dataTypeProv, variableProv, functionProv, operatorProv, parent);
                    if (importNodeR.Failed)
                        return importNodeR;

                    var importNode = importNodeR.ASTNode;                    

                    try
                    {
                        ValidateAST(importNode);
                    }
                    catch(Exception e)
                    {
                        return BuildNodeResult.Error(e.Message);
                    }

                    Imports.Add((path, importNode));
                }                

                var node = new ImportNode(parent, path);
                return BuildNodeResult.Node(node);
            }
            else if(parseTreeNode is SyscallFunctionHeader syscallFunctionHeader)
            {
                var package = AbstractASTNode.GetPackage(parent);
                var scope = AbstractASTNode.GetScope(parent);

                var declArgs = syscallFunctionHeader.Arguments;
                var declOut = syscallFunctionHeader.DataType;


                List<DataType> argTypes = new List<DataType>();

                foreach (var arg in declArgs)
                {
                    var dt = dataTypeProv.Resolve(arg.DataType, scope, false);
                    if (dt == null)
                        return BuildNodeResult.Error($"Unknown type : {arg.DataType}");
                    argTypes.Add(dt);
                }


                var outDataType = dataTypeProv.Resolve(declOut, scope, false);

                if (outDataType == null)
                    return BuildNodeResult.Error($"Unknown type : {outDataType}");



                var syscall = new SyscallFunction(syscallFunctionHeader.SyscallId, syscallFunctionHeader.Name, package, scope,
                    argTypes.ToArray(), outDataType);

                var syscallNode = new FunctionDeclarationNode(parent) { Function = syscall };

                functionProv.Add(syscall);
                return BuildNodeResult.Node(syscallNode);
            }


            return BuildNodeResult.Error($"Unknown node : {parseTreeNode.GetType().Name}");
        }

        public List<(string Source, IASTNode Node)> Imports = new List<(string, IASTNode)>();

        public IImportResolver ImportResolver { get; set; }

        private BuildNodeResult ExpectExpression(BuildNodeResult node, out IExpressionNode expr)
        {
            expr = null;
            if (node.Failed)
                return node;
            if (!(node.ASTNode is IExpressionNode _expr))
                return BuildNodeResult.Error("Type expression required");
            expr = _expr;
            return node;
        }

        /*DataType ResolveDataType(Identifier identifier)
        {

        }*/

        private void ValidateAST(IASTNode ast)
        {
            AbstractASTNode.AssertAllNonNestedInType<FunctionDeclarationNode, ReturnNode>(ast,
                (fundecl, ret) => ret.OutputType == fundecl.OutputType,
                (fundecl, ret) => throw new ReturnTypeMismatchException(
                    $"Wrong return type '{ret.OutputType}' in function '{fundecl.Function.FullName}' which returns '{fundecl.OutputType}'")
                );

            AbstractASTNode.AssertAllNodes<IfNode>(ast, 
                node => node.Condition.OutputType == BooleanConstantType,
                nd => throw new ArgumentException("If conditional must be a boolean expression"));

            AbstractASTNode.AssertAllNodes<RepeatNNode>(ast,
                node => node.Count.OutputType == IntegerConstantType,
                nd => throw new ArgumentException("Repeat counter must be of integer type"));

            AbstractASTNode.AssertAllNodes<PackageNode>(ast,
                node => node.Parent == null || node.Parent is PackageNode,
                nd => throw new ArgumentException("Package node must be top-level statement or nested in another package"));
        }

        class BuildNodeResult
        {
            public IASTNode ASTNode { get; }
            public string ErrorMessage { get; }
            public bool Failed => ErrorMessage != null;

            private BuildNodeResult(IASTNode aSTNode, string error)
            {
                ASTNode = aSTNode;
                ErrorMessage = error;
            }

            public static BuildNodeResult Node(IASTNode node) => new BuildNodeResult(node, null);
            public static BuildNodeResult Error(string message) => new BuildNodeResult(null, message);
        }
    }
}
