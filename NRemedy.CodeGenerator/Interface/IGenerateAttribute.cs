using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.CodeGenerator
{
    public interface IGenerateAttribute
    {
        void CreateAttribute(CodeTypeMember targetType, List<AttributeStructure> Attributes);
        void CreateCLassAttribute(CodeTypeDeclaration classType, List<AttributeStructure> Attributes);
        void CreatePropertyAttribute(CodeMemberProperty propertyType, List<AttributeStructure> Attributes);
    }
}
