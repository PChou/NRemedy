using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.CodeGenerator
{
    public interface IGenerateClass
    {
        void Create(CodeNamespace rootNamespace, ClassStructure classDefinition, IGenerateAttribute attributeGenerator);
    }
}
