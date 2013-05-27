using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ARNative;

namespace NRemedy
{
    public class GenerateProperty : IGenerateProperty
    {
        public void CreateProperty(CodeTypeDeclaration classType, string propertyName, string propertyType, bool isnullable, Dictionary<string, object> attributePropertyValues, IGenerateAttribute attributeGenerator)
        {
            if (classType == null) throw new ArgumentNullException("classType");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");
            if (string.IsNullOrEmpty(propertyType)) throw new ArgumentNullException("propertyType");
            if (attributePropertyValues == null) throw new ArgumentNullException("attributePropertyValues");
            if (attributeGenerator == null) throw new ArgumentNullException("attributeGenerator");

            GenerateNameResolver gnr = new GenerateNameResolver();
            CodeMemberProperty _property = new CodeMemberProperty();
            _property.Name = propertyName;
            if (isnullable)
            {
                propertyType = "Nullable<" + propertyType + ">";
            }
            _property.Type = new CodeTypeReference(propertyType);
            _property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            string fieldName = propertyName;
            _property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
            _property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));
            _property.SetStatements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "OnPropertyChanged"), new CodeExpression[] { new CodePrimitiveExpression(propertyName) }));

            //add attribute
            attributeGenerator.CreatePropertyAttribute(_property, typeof(ARFieldAttribute), attributePropertyValues);
            if (attributePropertyValues["DatabaseID"].ToString() == "1")
            {
                attributeGenerator.CreatePropertyAttribute(_property, typeof(AREntryKeyAttribute), null);
            }
            classType.Members.Add(_property);
        }
        public void CreateSelectionProperty(CodeTypeDeclaration classType, string propertyName, string propertyType, bool isnullable, ARFieldLimit limit, Dictionary<string, object> attributePropertyValues, IGenerateAttribute attributeGenerator)
        {
            if (limit == null) throw new ArgumentNullException("ARFieldLimit");

            CreateProperty(classType, propertyName, propertyType, isnullable, attributePropertyValues, attributeGenerator);

            //add enum
            AREnumList arenumlist = limit.enumlimit;
            if (arenumlist != null)
            {
                List<AREnum> enumList = arenumlist.enumList;
                CodeTypeDeclaration enCode = new CodeTypeDeclaration(propertyType);
                enCode.IsEnum = true;
                enCode.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                for (int i = 0; i < enumList.Count; i++)
                {
                    string value = enumList[i].itemName.ToString();
                    value = Regex.Replace(value, @"[^A-Za-z0-9_]", "");
                    if (Regex.IsMatch(value, @"^\d+"))
                    {
                        value = "_" + value;
                    }
                    CodeMemberField mem = new CodeMemberField(typeof(Int32), value);
                    mem.Attributes = MemberAttributes.Public;
                    mem.InitExpression = new CodeSnippetExpression(enumList[i].itemNumber.ToString());
                    enCode.Members.Add(mem);
                }
                classType.Members.Add(enCode);
            }

        }

        public void CreateDiaryProperty(CodeTypeDeclaration classType, string propertyName, string propertyType, bool isnullable, Dictionary<string, object> attributePropertyValues, IGenerateAttribute attributeGenerator)
        {
            
        }
    }
}
