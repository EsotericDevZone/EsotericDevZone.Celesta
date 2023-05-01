using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Celesta.Parser.ParseTree;
using System;
using System.Collections.Generic;

namespace EsotericDevZone.Celesta.Providers
{
    public interface ISymbolProvider<T> where T : ISymbol
    {
        /// <summary>
        /// Finds symbols by name which are visible in the scope
        /// </summary>        
        /// <returns>list of found symbols</returns>
        IEnumerable<T> Find(string name, string scope);

        /// <summary>
        /// Finds symbols by name and package which are visible in the scope
        /// </summary>
        /// <returns>list of found symbols</returns>
        IEnumerable<T> Find(string package, string name, string scope);

        /// <summary>
        /// Finds symbols by identifier which are visible in the scope
        /// </summary>
        /// <returns>list of found symbols</returns>
        IEnumerable<T> Find(Identifier identifier, string scope);
        void Add(T symbol);
        IEnumerable<T> GetAll();

        IEnumerable<T> Where(Func<T, bool> predicate);
        void Clear();
        void AddFromProvider(ISymbolProvider<T> provider);
        void CopyFromProvider(ISymbolProvider<T> provider);

        /// <summary>
        /// Finds a symbol definition based on an identifier
        /// </summary>
        /// <param name="identifier">symbol identifier</param>
        /// <param name="scope">target scope</param>
        /// <param name="strict">in case of multiple definitions, if "strict" is true, an exception is thrown, otherwise, 
        ///     the bottom most symbol in the scope tree is returned
        /// </param>
        /// <returns>symbol definition which fits the requirements, or null if no such symbol exists</returns>
        T Resolve(Identifier identifier, string scope, bool strict);        
    }

    public interface IDataTypeProvider : ISymbolProvider<DataType> { }
    public interface IVariableProvider : ISymbolProvider<Variable> { }
    public interface IFunctionProvider : ISymbolProvider<Function> 
    {
        IEnumerable<Function> Find(string name, string scope, DataType[] argTypes);
        IEnumerable<Function> Find(string name, string scope, DataType[] argTypes, DataType outputType);
        IEnumerable<Function> Find(string package, string name, string scope, DataType[] argTypes);
        IEnumerable<Function> Find(string package, string name, string scope, DataType[] argTypes, DataType outputType);
    }

    public interface IOperatorProvider : ISymbolProvider<Operator>
    {
        IEnumerable<Operator> FindUnary(string name, DataType argType1);
        IEnumerable<Operator> FindUnary(string name, DataType argType1, DataType outputType);
        IEnumerable<Operator> FindBinary(string name, DataType argType1, DataType argType2);
        IEnumerable<Operator> FindBinary(string name, DataType argType1, DataType argType2, DataType outputType);
    }
}
