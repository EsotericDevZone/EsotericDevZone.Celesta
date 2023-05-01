using EsotericDevZone.Celesta.Counters;
using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.Celesta.Providers;
using EsotericDevZone.Core;
using EsotericDevZone.Core.Collections;
using EsotericDevZone.RuleBasedParser;
using System;
using System.Linq;
using System.Xml.Linq;

namespace EsotericDevZone.Celesta.AST
{
    public class ASTBuilder
    {       
        public static ASTBuilder GetDebug()
        {
            var dtprov = new DataTypeProvider();
            var vbprov = new VariableProvider();
            var fnprov = new FunctionProvider();
            var opprov = new OperatorProvider();

            var Int = DataType.Primitive("int", "", "@main");            

            dtprov.Add(Int);
            dtprov.Add(DataType.Primitive("decimal", "", "@main"));
            dtprov.Add(DataType.Primitive("string", "", "@main"));

            opprov.Add(Operator.BinaryOperator("+", Int, Int, Int));
            opprov.Add(Operator.BinaryOperator("-", Int, Int, Int));
            opprov.Add(Operator.UnaryOperator("-", Int, Int));

            var astb = new ASTBuilder(dtprov, vbprov, fnprov, opprov);

            astb.IntegerConstantType = dtprov.Find("int", "@main").First();
            astb.RealConstantType = dtprov.Find("decimal", "@main").First();
            astb.StringConstantType = dtprov.Find("string", "@main").First();

            return astb;
        }

        public DataType IntegerConstantType { get; internal set; }
        public DataType RealConstantType { get; internal set; }
        public DataType StringConstantType { get; internal set; }


        private IDataTypeProvider DataTypeProvider;
        private IVariableProvider VariableProvider;
        private IFunctionProvider FunctionProvider;
        private IOperatorProvider OperatorProvider;

        private IStringCounter ScopeCounter = new FunctionalStringCounter(i => $"@_s{i}");

        public ASTBuilder(IDataTypeProvider dataTypeProvider, IVariableProvider variableProvider
            , IFunctionProvider functionProvider, IOperatorProvider operatorProvider)
        {
            DataTypeProvider = dataTypeProvider;
            VariableProvider = variableProvider;
            FunctionProvider = functionProvider;
            OperatorProvider = operatorProvider;
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

            if(buildResult.Failed)
            {
                throw new ASTBuildNodeException(buildResult.ErrorMessage);
            }

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
                Console.WriteLine("Val = " + strValue);
                var node = new StringConstantNode(parent, strValue.FromLiteral(), StringConstantType);
                return BuildNodeResult.Node(node);
            }            
            if (parseTreeNode is VariableDeclaration varDeclaraction)
            {
                var varName = varDeclaraction.VariableName;
                var typeIdentifier = varDeclaraction.DataType;
                var scope = AbstractASTNode.GetScope(parent);

                var dataType = dataTypeProv.Resolve(typeIdentifier, scope, false);
                if (dataType == null)
                    return BuildNodeResult.Error($"Unidentified data type : {typeIdentifier.FullName}");                    

                var package = AbstractASTNode.GetPackage(parent);

                if(variableProv.Find(package, varName, scope).Count()>0)
                {
                    return BuildNodeResult.Error($"Duplicate variable definition : {varName}");
                }

                var variable = new Variable(varName, package, scope, dataType);

                var decl = new VariableDeclarationNode(parent);

                var variableNode = new VariableNode(decl, variable);
                decl.VariableNode = variableNode;

                var assignedExpression = BuildNodeRec(varDeclaraction.InitializationValue
                    , dataTypeProv, variableProv, functionProv, operatorProv, decl);

                if (assignedExpression.Failed)
                    return assignedExpression;

                decl.AssignedExpression = assignedExpression.ASTNode;

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


            return BuildNodeResult.Error($"Unknown node : {parseTreeNode.GetType().Name}");
        }

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
