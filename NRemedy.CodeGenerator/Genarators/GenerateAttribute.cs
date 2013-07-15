using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.CodeGenerator
{
    public class GenerateAttribute : IGenerateAttribute
    {
        public void CreateAttribute(CodeTypeMember targetType, List<AttributeStructure> Attributes)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");
            if (Attributes == null) throw new ArgumentNullException("Attributes");
            foreach (AttributeStructure Attribute in Attributes)
            {
                CodeTypeReference _attributeType = new CodeTypeReference(Attribute.AttributeType);
                CodeAttributeDeclaration _cad = new CodeAttributeDeclaration(_attributeType);
                if (Attribute.AttributeArguments != null)
                {
                    foreach (var propertry in Attribute.AttributeArguments)
                    {
                        object value = propertry.Value;
                        CodeAttributeArgument _arg = null;
                        if (value.GetType().IsEnum)
                            _arg = new CodeAttributeArgument(propertry.Key, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(value.GetType().Name), value.ToString()));
                        else
                            _arg = new CodeAttributeArgument(propertry.Key, new CodePrimitiveExpression(propertry.Value));
                        _cad.Arguments.Add(_arg);
                    }
                }
                targetType.CustomAttributes.Add(_cad);
            }
        }
        public void CreateCLassAttribute(CodeTypeDeclaration classType, List<AttributeStructure> Attributes)
        {
            CreateAttribute((CodeTypeMember)classType, Attributes);
        }
        public void CreatePropertyAttribute(CodeMemberProperty propertyType, List<AttributeStructure> Attributes)
        {
            CreateAttribute((CodeTypeMember)propertyType, Attributes);
        }
    }
}
