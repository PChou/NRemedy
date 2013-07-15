using System;
using System.CodeDom;
using System.Collections.Generic;

namespace NRemedy.CodeGenerator
{
    public class GenerateImportNamespace : IGenerateImportNamespace
    {
        public void Create(CodeNamespace rootNamespace, List<NamespaceStructure> namespaceImports)
        {
            if (rootNamespace == null) throw new ArgumentNullException("baseNamespace");

            foreach (NamespaceStructure n in namespaceImports)
            {
                rootNamespace.Imports.Add(new CodeNamespaceImport(n.NamespaceName));
            }
        }   
    }
}
