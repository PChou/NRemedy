using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.CodeGenerator
{
    public class GenerateField : IGenerateField
    {
        public void CreateField(CodeTypeDeclaration classType, FieldStructure MemberField)
        {
            if (classType == null) throw new ArgumentNullException("classType");
            if (MemberField == null) throw new ArgumentNullException("MemberField");
            if (string.IsNullOrEmpty(MemberField.FieldName)) throw new ArgumentNullException("FieldName");
            if (MemberField.FieldType == null) throw new ArgumentNullException("FieldType");


            CodeMemberField _field = new CodeMemberField();
            _field.Name = MemberField.FieldName;

            _field.Type = new CodeTypeReference(MemberField.FieldTypeName);
            _field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
            classType.Members.Add(_field);
        }

        public void CreateSelectionField(CodeTypeDeclaration classType, FieldStructure MemberField)
        {
            CreateField(classType, MemberField);
        }

        public void CreateDiaryField(CodeTypeDeclaration classType, FieldStructure MemberField)
        {
            
        }
    }
}
