using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy
{
    public class GenerateRootNamespace : IGenerateRootNamespace
    {
        private string __namespace = "NRemedy";

        public void Create(CodeCompileUnit codeCompileUnit)
        {
            if (codeCompileUnit == null) throw new ArgumentNullException("codeCompileUnit");
            codeCompileUnit.Namespaces.Add(new CodeNamespace(__namespace));
        }
    }
}
