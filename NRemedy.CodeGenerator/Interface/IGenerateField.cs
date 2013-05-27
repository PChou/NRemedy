using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy
{
    public interface IGenerateField
    {
        void CreateField(CodeTypeDeclaration classType, string fieldName, string fieldType, bool isnullable);
        void CreateSelectionField(CodeTypeDeclaration classType, string fieldName, string fieldType, bool isnullable);
        void CreateDiaryField(CodeTypeDeclaration classType, string fieldName, string fieldType, bool isnullable);
    }
}
