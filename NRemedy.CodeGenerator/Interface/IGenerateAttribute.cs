using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy
{
    public interface IGenerateAttribute
    {
        void CreateAttribute(CodeTypeMember targetType, Type attributeType, Dictionary<string, object> properties);
        void CreateCLassAttribute(CodeTypeDeclaration classType, Type attributeType, Dictionary<string, object> properties);
        void CreatePropertyAttribute(CodeMemberProperty propertyType, Type attributeType, Dictionary<string, object> properties);
    }
}
