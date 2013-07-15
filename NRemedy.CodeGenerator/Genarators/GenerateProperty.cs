using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ARNative;

namespace NRemedy.CodeGenerator
{
    public class GenerateProperty : IGenerateProperty
    {
        public void CreateProperty(CodeTypeDeclaration classType, PropertyStructure Property, IGenerateAttribute attributeGenerator)
        {
            if (classType == null) throw new ArgumentNullException("classType");
            if (Property == null) throw new ArgumentNullException("Property");
            if (string.IsNullOrEmpty(Property.PropertyName)) throw new ArgumentNullException("propertyName");
            if (Property.PropertyType == null) throw new ArgumentNullException("propertyType");
            if (attributeGenerator == null) throw new ArgumentNullException("attributeGenerator");

            GenerateNameResolver gnr = new GenerateNameResolver();
            CodeMemberProperty _property = new CodeMemberProperty();
            _property.Name = Property.PropertyName;
            _property.Type = new CodeTypeReference(Property.PropertyTypeName);
            _property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            string fieldName = Property.MemberField.FieldName;
            _property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
            _property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));
            _property.SetStatements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "OnPropertyChanged"), new CodeExpression[] { new CodePrimitiveExpression(Property.PropertyName) }));

            //add attribute
            attributeGenerator.CreatePropertyAttribute(_property, Property.PropertyAttributeList);
            foreach(AttributeStructure Attribute in Property.PropertyAttributeList)
            {
                if (Attribute.AttributeArguments["DatabaseID"] != null && Attribute.AttributeArguments["DatabaseID"].ToString() == "1")
                {
                    AttributeStructure entrykeyattr = new AttributeStructure();
                    entrykeyattr.AttributeType = typeof(AREntryKeyAttribute);
                    List<AttributeStructure> AttrList = new List<AttributeStructure>();
                    AttrList.Add(entrykeyattr);
                    attributeGenerator.CreatePropertyAttribute(_property, AttrList);
                }
            }
            classType.Members.Add(_property);
        }
        public void CreateSelectionProperty(CodeTypeDeclaration classType, PropertyStructure Property, IGenerateAttribute attributeGenerator)
        {
            if (Property.SelectionLimit == null) throw new ArgumentNullException("ARFieldLimit");

            CreateProperty(classType, Property, attributeGenerator);

            //add enum
            AREnumList arenumlist = Property.SelectionLimit.enumlimit;
            if (arenumlist != null)
            {
                List<AREnum> enumList = arenumlist.enumList;
                CodeTypeDeclaration enCode = new CodeTypeDeclaration(Property.PropertyTypeName);
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

        public void CreateDiaryProperty(CodeTypeDeclaration classType, PropertyStructure Property, IGenerateAttribute attributeGenerator)
        {
            
        }
    }
}
