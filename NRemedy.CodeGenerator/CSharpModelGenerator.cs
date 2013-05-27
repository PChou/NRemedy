using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;

namespace NRemedy
{
    public class CSharpModelGenerator
    {
        private CodeGeneratorOptions _options;
        private ARLoginContext _context;
        public CSharpModelGenerator(ARLoginContext context)
        {
            _context = context;
            _options = new CodeGeneratorOptions();
            _options.BlankLinesBetweenMembers = false;
            _options.IndentString = "\t";
        }
        public CodeCompileUnit GenerateModelCCU(string FormName, CSharpModelGeneratorDefaultFactory factory)
        {
            CodeCompileUnit ccu = new CodeCompileUnit();
            IARSchema ar = factory.CreateARSchema(_context);
            IGenerateNameResolver gnr = factory.CreateGenerateNameResolver();
            //rootnamespace
            IGenerateRootNamespace g_np = factory.CreateGenerateRootNamespace();
            //importnamespace
            IGenerateImportNamespace g_inp = factory.CreateGenerateImportNamespace();
            //attribute
            IGenerateAttribute g_attri = factory.CreateGenerateAttribute();
            //class
            IGenerateClass g_class = factory.CreateGenerateClass();
            g_np.Create(ccu);
            g_inp.Create(ccu.Namespaces[0]);
            g_class.Create(ccu.Namespaces[0], gnr.DefaultClassNameResolver(FormName), FormName, g_attri);
            //get field list for generate code from AR
            ARFieldFilterDelegate filter = factory.CreateARFieldFilterDelegate();
            List<ARField> fields = ar.GetListFieldWithDetail(FormName, filter);
            //field
            IGenerateField g_fields = factory.CreateGenerateField();
            foreach (ARField f1 in fields)
            {
                Type type = ARTypeConvert.MappingARType((ARType)f1.dataType);
                if (type != null)
                {
                    string fieldType = type.Name;
                    string fieldName = gnr.DefaultFieldNameResolver(f1.fieldName);
                    if ((ARType)f1.dataType == ARType.SelectionField)
                    {
                        fieldType = gnr.SelectionTypeResolver(f1.fieldName);
                        bool isnullable = f1.option != 1 && type.IsValueType;
                        g_fields.CreateSelectionField(ccu.Namespaces[0].Types[0], fieldName, fieldType, isnullable);
                    }
                    else
                    {
                        bool isnullable = f1.option != 1 && type.IsValueType;
                        g_fields.CreateField(ccu.Namespaces[0].Types[0], fieldName, fieldType, isnullable);
                    }
                }
            }
            //property
            IGenerateProperty g_property = factory.CreateGenerateProperty();
            Dictionary<string, object> mapping = new Dictionary<string, object>();
            foreach (ARField f2 in fields)
            {
                Type type = ARTypeConvert.MappingARType((ARType)f2.dataType);
                if (type != null)
                {
                    mapping.Clear();
                    mapping.Add("DatabaseID", f2.fieldId);
                    mapping.Add("DatabaseName", f2.fieldName);
                    mapping.Add("DataType", (ARType)f2.dataType);
                    string propertyType = type.Name;
                    string propertyName = gnr.DefaultPropertyNameResolver(f2.fieldName);
                    if ((ARType)f2.dataType == ARType.SelectionField)
                    {
                        propertyType = gnr.SelectionTypeResolver(f2.fieldName);
                        bool isnullable = f2.option != 1 && type.IsValueType;
                        g_property.CreateSelectionProperty(ccu.Namespaces[0].Types[0], propertyName, propertyType, isnullable, f2.limit, mapping, g_attri);
                    }
                    else
                    {
                        bool isnullable = f2.option != 1 && type.IsValueType;
                        g_property.CreateProperty(ccu.Namespaces[0].Types[0], propertyName, propertyType, isnullable, mapping, g_attri);
                    }
                }
            }
            return ccu;
        }
    }
}
