﻿using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Celesta.Definitions.Functions;
using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace EsotericDevZone.Celesta.Providers
{
    public class FunctionProvider : AbstractSymbolProvider<Function>, IFunctionProvider
    {
        public Function GetBySyscallId(int syscallId)
        {
            return Where(f => f is SyscallFunction sc && sc.SyscallId == syscallId).FirstOrDefault();
        }

        public IEnumerable<Function> Find(string name, string scope, DataType[] argTypes)
        {
            return Find(name, scope).Where(f=>f.ArgumentTypes.SequenceEqual(argTypes));
        }

        public IEnumerable<Function> Find(string name, string scope, DataType[] argTypes, DataType outputType)
        {
            return Find(name, scope).Where(f => f.ArgumentTypes.SequenceEqual(argTypes)
             && f.OutputType == outputType);
        }

        public IEnumerable<Function> Find(string package, string name, string scope, DataType[] argTypes)
        {
            return Find(package, name, scope).Where(f => f.ArgumentTypes.SequenceEqual(argTypes));
        }

        public IEnumerable<Function> Find(string package, string name, string scope, DataType[] argTypes, DataType outputType)
        {
            return Find(package, name, scope).Where(f => f.ArgumentTypes.SequenceEqual(argTypes)
             && f.OutputType == outputType);
        }

        public Function Resolve(Identifier identifier, string scope, DataType[] argTypes, bool strict)
        {
            Function[] candidates;
            if (identifier.PackageName == "")
                candidates = Find(identifier.Name, scope, argTypes).ToArray();
            else
                candidates = Find(identifier.PackageName, identifier.Name, scope, argTypes).ToArray();
            if (candidates.Length == 0)
                return null;            
            if (strict && candidates.Length > 1)
                throw new MultipleDefinitionException($"Multiple definitions of function {identifier}({argTypes.JoinToString(",")}):\n{candidates.JoinToString("\n").Indent("  ")}");
            return candidates[0];
        }

        public Function Fit(Identifier identifier, string scope, DataType[] argTypes, bool strict)
        {            
            Function[] candidates;
            if (identifier.PackageName == "")
                candidates = Find(identifier.Name, scope).ToArray();
            else
                candidates = Find(identifier.PackageName, identifier.Name, scope).ToArray();
            candidates = candidates
                .Where(f => argTypes.Select((t, i) => (t, i)).All(p => p.i<f.ArgumentTypes.Length && p.t.Substitutes(f.ArgumentTypes[p.i])))                
                .Where(f=>f.ArgumentTypes.Length == argTypes.Length)                
                .ToArray();            
            if (candidates.Length == 0)
                return null;
            if (strict && candidates.Length > 1)
                throw new MultipleDefinitionException($"Multiple definitions of function {identifier}({argTypes.JoinToString(",")}):\n{candidates.JoinToString("\n").Indent("  ")}");            
            return candidates[0];
        }
    }
}
