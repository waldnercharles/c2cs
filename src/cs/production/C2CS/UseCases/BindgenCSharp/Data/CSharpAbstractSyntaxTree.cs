// Copyright (c) Lucas Girouard-Stranks (https://github.com/lithiumtoast). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;

namespace C2CS.UseCases.BindgenCSharp
{
    public readonly struct CSharpAbstractSyntaxTree
    {
        public readonly ImmutableArray<CSharpVariable> Variables;

        public readonly ImmutableArray<CSharpFunction> Functions;

        public readonly ImmutableArray<CSharpFunctionPointer> FunctionPointers;

        public readonly ImmutableArray<CSharpStruct> Structs;

        public readonly ImmutableArray<CSharpTypedef> Typedefs;

        public readonly ImmutableArray<CSharpOpaqueType> OpaqueDataTypes;

        public readonly ImmutableArray<CSharpEnum> Enums;

        public readonly ImmutableArray<CSharpPseudoEnum> PseudoEnums;

        public readonly ImmutableArray<CSharpConstant> Constants;

        public CSharpAbstractSyntaxTree(
            ImmutableArray<CSharpVariable> variables,
            ImmutableArray<CSharpFunction> functions,
            ImmutableArray<CSharpFunctionPointer> functionPointers,
            ImmutableArray<CSharpStruct> structs,
            ImmutableArray<CSharpTypedef> typedefs,
            ImmutableArray<CSharpOpaqueType> opaqueDataTypes,
            ImmutableArray<CSharpEnum> enums,
            ImmutableArray<CSharpPseudoEnum> pseudoEnums,
            ImmutableArray<CSharpConstant> constants)
        {
            Variables = variables;
            Functions = functions;
            FunctionPointers = functionPointers;
            Structs = structs;
            Typedefs = typedefs;
            OpaqueDataTypes = opaqueDataTypes;
            Enums = enums;
            PseudoEnums = pseudoEnums;
            Constants = constants;
        }
    }
}
