using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;

namespace NRemedy.CodeGenerator
{
    public class CodeStructure
    {
        public NamespaceStructure RootNamespace { get; set; }
        public List<NamespaceStructure> ImportNamespaceList { get; set; }
        public ClassStructure ClassDefinition { get; set; }
    }

    public class NamespaceStructure
    {
        public String NamespaceName { get; set; }
    }

    public class AttributeStructure
    {
        public Type AttributeType { get; set; }
        public Dictionary<string, object> AttributeArguments { get; set; }
    }

    public class ClassStructure
    {
        public String ClassName { get; set; }
        public List<AttributeStructure> ClassAttributeList { get; set; }
        public List<PropertyStructure> PropertyList { get; set; }
    }

    public class PropertyStructure
    {
        public String PropertyName { get; set; }
        public String PropertyTypeName { get; set; }
        public ARType PropertyType { get; set; }
        public bool IsNullable { get; set; }
        public List<AttributeStructure> PropertyAttributeList { get; set; }
        public FieldStructure MemberField { get; set; }
        public ARFieldLimit SelectionLimit { get; set; }
    }

    public class FieldStructure
    {
        public String FieldName { get; set; }
        public String FieldTypeName { get; set; }
        public ARType FieldType { get; set; }
        public bool IsNullable { get; set; }
    }
}
