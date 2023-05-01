﻿using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    internal class AbstractExpressionNode : AbstractASTNode, IExpressionNode
    {
        public DataType OutputType { get; }

        public AbstractExpressionNode(IASTNode parent, DataType outputType) : base(parent)
        {
            OutputType = outputType;
        }
    }
}
