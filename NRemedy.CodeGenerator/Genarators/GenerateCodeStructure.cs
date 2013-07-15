using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;

namespace NRemedy.CodeGenerator
{
    public class GenerateCodeStructure : IGenerateCodeStructure
    {
        private string __namespace = "NRemedy";
        private string[] __namespaceImport = {"System" , 
                                                 "System.Text" , 
                                                 "System.Collections.Generic",
                                                 "NRemedy",
                                             };
        public CodeStructure Create(string FormName, List<ARField> Fields, IGenerateNameResolver gnr)
        {
            CodeStructure CodeStructureDefinition = new CodeStructure();
            NamespaceStructure RootNamespace = new NamespaceStructure();
            RootNamespace.NamespaceName = __namespace;
            CodeStructureDefinition.RootNamespace = RootNamespace;
            List<NamespaceStructure> ImportNamespaceList = new List<NamespaceStructure>();
            foreach (var item in __namespaceImport)
            {
                NamespaceStructure ImportNamespace = new NamespaceStructure();
                ImportNamespace.NamespaceName = item;
                ImportNamespaceList.Add(ImportNamespace);
            }
            CodeStructureDefinition.ImportNamespaceList = ImportNamespaceList;

            ClassStructure ClassDefinition = new ClassStructure();
            ClassDefinition.ClassName = gnr.DefaultClassNameResolver(FormName);
            List<AttributeStructure> ClassAttributeList = new List<AttributeStructure>();
            AttributeStructure ClassAttribute = new AttributeStructure();
            ClassAttribute.AttributeType = typeof(ARFormAttribute);
            ClassAttribute.AttributeArguments = new Dictionary<string, object> { {"FormName",FormName}};
               
            ClassAttributeList.Add(ClassAttribute);
            ClassDefinition.ClassAttributeList = ClassAttributeList;

            List<PropertyStructure> PropertyList = new List<PropertyStructure>();
            foreach (var field in Fields)
            {
                Type type = ARTypeConvert.MappingARType((ARType)field.dataType);
                bool isnullable = field.option != 1 && type.IsValueType;
                PropertyStructure PropertyDefinition = new PropertyStructure();
                PropertyDefinition.PropertyName = gnr.DefaultPropertyNameResolver(field.fieldName);
                PropertyDefinition.PropertyType = (ARType)field.dataType;
                PropertyDefinition.IsNullable = isnullable;
                FieldStructure MemberField = new FieldStructure();
                MemberField.FieldName = gnr.DefaultFieldNameResolver(field.fieldName);
                MemberField.FieldType = (ARType)field.dataType;
                MemberField.IsNullable = isnullable;
                if ((ARType)field.dataType == ARType.SelectionField)
                {
                    MemberField.FieldTypeName = gnr.SelectionTypeResolver(field.fieldName);
                    PropertyDefinition.PropertyTypeName = gnr.SelectionTypeResolver(field.fieldName);
                    PropertyDefinition.SelectionLimit = field.limit;
                }
                else
                {
                    MemberField.FieldTypeName = type.Name;
                    PropertyDefinition.PropertyTypeName = type.Name;
                }
                if (isnullable)
                {
                    MemberField.FieldTypeName = "Nullable<" + MemberField.FieldTypeName + ">";
                    PropertyDefinition.PropertyTypeName = "Nullable<" + PropertyDefinition.PropertyTypeName + ">";
                }
                PropertyDefinition.MemberField = MemberField;
                List<AttributeStructure> PropertyAttributeList = new List<AttributeStructure>();
                AttributeStructure PropertyAttribute = new AttributeStructure();
                PropertyAttribute.AttributeType = typeof(ARFieldAttribute);
                Dictionary<string, object> mapping = new Dictionary<string, object>();
                mapping.Add("DatabaseID", field.fieldId);
                mapping.Add("DatabaseName", field.fieldName);
                mapping.Add("DataType", (ARType)field.dataType);
                PropertyAttribute.AttributeArguments = mapping;
                PropertyAttributeList.Add(PropertyAttribute);
                PropertyDefinition.PropertyAttributeList = PropertyAttributeList;
                PropertyList.Add(PropertyDefinition);
            }
            ClassDefinition.PropertyList = PropertyList;
            CodeStructureDefinition.ClassDefinition = ClassDefinition;
            return CodeStructureDefinition;
        }
    }
}
