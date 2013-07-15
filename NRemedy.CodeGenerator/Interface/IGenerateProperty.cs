using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;

namespace NRemedy.CodeGenerator
{
    public interface IGenerateProperty
    {
        void CreateProperty(CodeTypeDeclaration classType, PropertyStructure Property, IGenerateAttribute attributeGenerator);
        void CreateSelectionProperty(CodeTypeDeclaration classType, PropertyStructure Property, IGenerateAttribute attributeGenerator);
        void CreateDiaryProperty(CodeTypeDeclaration classType, PropertyStructure Property, IGenerateAttribute attributeGenerator);
    }
}
