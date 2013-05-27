using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy
{
    public class GenerateField : IGenerateField
    {
        public void CreateField(CodeTypeDeclaration classType, string fieldName, string fieldType, bool isnullable)
        {
            if (classType == null) throw new ArgumentNullException("classType");
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentNullException("fieldName");
            if (string.IsNullOrEmpty(fieldType)) throw new ArgumentNullException("fieldType");


            CodeMemberField _field = new CodeMemberField();
            _field.Name = fieldName;
            if (isnullable)
            {
                fieldType = "Nullable<" + fieldType + ">";
            }
            _field.Type = new CodeTypeReference(fieldType);
            _field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
            classType.Members.Add(_field);
        }

        public void CreateSelectionField(CodeTypeDeclaration classType, string fieldName, string fieldType, bool isnullable)
        {
            CreateField(classType, fieldName, fieldType, isnullable);
        }

        public void CreateDiaryField(CodeTypeDeclaration classType, string fieldName, string fieldType, bool isnullable)
        {
            
        }
    }
}
