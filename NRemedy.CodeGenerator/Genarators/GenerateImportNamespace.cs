using System;
using System.CodeDom;

namespace NRemedy
{
    public class GenerateImportNamespace : IGenerateImportNamespace
    {
        private string[] __namespaceImport = {"System" , 
                                                 "System.Text" , 
                                                 "System.Collections.Generic",
                                                 "NRemedy",
                                             };

        public void Create(CodeNamespace rootNamespace)
        {
            if (rootNamespace == null) throw new ArgumentNullException("baseNamespace");
            foreach (string n in __namespaceImport){
                rootNamespace.Imports.Add(new CodeNamespaceImport(n));
            }
        }
    }
}
