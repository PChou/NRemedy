using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy
{
    public interface IGenerateRootNamespace
    {
        void Create(CodeCompileUnit codeCompileUnit);
    }
}
