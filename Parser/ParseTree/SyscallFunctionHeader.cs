using EsotericDevZone.Core;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class SyscallFunctionHeader : IParseTreeNode
    {
        public int SyscallId { get; }
        public string Name { get; }
        public FunctionArgumentDeclaration[] Arguments { get; }
        public Identifier DataType { get; }

        public SyscallFunctionHeader(int syscallId, string name, FunctionArgumentDeclaration[] arguments, Identifier dataType)
        {
            SyscallId = syscallId;
            Name = name;
            Arguments = arguments;
            DataType = dataType;
        }

        public override string ToString()
            => $"syscall {SyscallId} {DataType} {Name}({Arguments.JoinToString(",")})";
    }
}
