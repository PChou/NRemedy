using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.CodeGenerator
{
    public interface IGenerateField
    {
        void CreateField(CodeTypeDeclaration classType, FieldStructure MemberField);
        void CreateSelectionField(CodeTypeDeclaration classType, FieldStructure MemberField);
        void CreateDiaryField(CodeTypeDeclaration classType, FieldStructure MemberField);
    }
}
