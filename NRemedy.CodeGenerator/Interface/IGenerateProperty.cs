using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;

namespace NRemedy
{
    public interface IGenerateProperty
    {
        void CreateProperty(CodeTypeDeclaration classType, string propertyName, string propertyType, bool isnullable, Dictionary<string, object> attributePropertyValues, IGenerateAttribute attributeGenerator);
        void CreateSelectionProperty(CodeTypeDeclaration classType, string propertyName, string propertyType, bool isnullable, ARFieldLimit limit, Dictionary<string, object> attributePropertyValues, IGenerateAttribute attributeGenerator);
        void CreateDiaryProperty(CodeTypeDeclaration classType, string propertyName, string propertyType, bool isnullable, Dictionary<string, object> attributePropertyValues, IGenerateAttribute attributeGenerator);
    }
}
