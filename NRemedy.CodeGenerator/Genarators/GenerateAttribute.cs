using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy
{
    public class GenerateAttribute : IGenerateAttribute
    {
        public void CreateAttribute(CodeTypeMember targetType, Type attributeType, Dictionary<string, object> properties)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");
            if (attributeType == null) throw new ArgumentNullException("attributeType");
            CodeTypeReference _attributeType = new CodeTypeReference(attributeType);
            CodeAttributeDeclaration _cad = new CodeAttributeDeclaration(_attributeType);
            if (properties != null)
            {
                foreach (var propertry in properties)
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
        public void CreateCLassAttribute(CodeTypeDeclaration classType, Type attributeType, Dictionary<string, object> properties)
        {
            CreateAttribute((CodeTypeMember)classType, attributeType, properties);
        }
        public void CreatePropertyAttribute(CodeMemberProperty propertyType, Type attributeType, Dictionary<string, object> properties)
        {
            CreateAttribute((CodeTypeMember)propertyType, attributeType, properties);
        }
    }
}
