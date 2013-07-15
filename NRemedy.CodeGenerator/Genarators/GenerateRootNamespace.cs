using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.CodeGenerator
{
    public class GenerateRootNamespace : IGenerateRootNamespace
    {
        public void Create(CodeCompileUnit codeCompileUnit, NamespaceStructure rootNamespace)
        {
            if (codeCompileUnit == null) throw new ArgumentNullException("codeCompileUnit");
            if (rootNamespace == null) throw new ArgumentNullException("rootNamespace");
            if (string.IsNullOrEmpty(rootNamespace.NamespaceName)) throw new ArgumentNullException("rootNamespace.NamespaceName");
            codeCompileUnit.Namespaces.Add(new CodeNamespace(rootNamespace.NamespaceName));
        }
    }
}
