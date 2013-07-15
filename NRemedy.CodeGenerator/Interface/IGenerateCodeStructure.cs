using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;

namespace NRemedy.CodeGenerator
{
    public interface IGenerateCodeStructure
    {
        CodeStructure Create(string FormName, List<ARField> Fields, IGenerateNameResolver gnr);
    }
}
