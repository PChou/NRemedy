using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy
{
    public interface IGenerateClass
    {
        void Create(CodeNamespace rootNamespace, string className, string formName, IGenerateAttribute attributeGenerator);
    }
}
