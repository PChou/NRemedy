using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;

namespace NRemedy.CodeGenerator
{
    public class CSharpModelGenerator
    {
        public CodeCompileUnit GenerateModelCCU(string FormName, List<ARField> Fields, CSharpModelGeneratorDefaultFactory factory)
        {
            CodeCompileUnit ccu = new CodeCompileUnit();

            IGenerateNameResolver gnr = factory.CreateGenerateNameResolver();

            IGenerateCodeStructure g_cs = factory.CreateGenerateDefaultCodeStructure();

            CodeStructure CodeStructureDefinition = g_cs.Create(FormName, Fields, gnr);
            //rootnamespace
            IGenerateRootNamespace g_np = factory.CreateGenerateRootNamespace();
            //importnamespace
            IGenerateImportNamespace g_inp = factory.CreateGenerateImportNamespace();
            //attribute
            IGenerateAttribute g_attri = factory.CreateGenerateAttribute();
            //class
            IGenerateClass g_class = factory.CreateGenerateClass();
            g_np.Create(ccu, CodeStructureDefinition.RootNamespace);
            g_inp.Create(ccu.Namespaces[0], CodeStructureDefinition.ImportNamespaceList);
            g_class.Create(ccu.Namespaces[0], CodeStructureDefinition.ClassDefinition, g_attri);

            //field
            IGenerateField g_fields = factory.CreateGenerateField();
            foreach (PropertyStructure property in CodeStructureDefinition.ClassDefinition.PropertyList)
            {
                FieldStructure field = property.MemberField;
                if (field != null)
                {
                    if (field.FieldType == ARType.SelectionField)
                    {
                        g_fields.CreateSelectionField(ccu.Namespaces[0].Types[0], field);
                    }
                    else
                    {
                        g_fields.CreateField(ccu.Namespaces[0].Types[0], field);
                    }
                }
            }
            //property
            IGenerateProperty g_property = factory.CreateGenerateProperty();
            foreach (PropertyStructure property in CodeStructureDefinition.ClassDefinition.PropertyList)
            {
                if (property != null)
                {
                    if ((ARType)property.PropertyType == ARType.SelectionField)
                    {
                        g_property.CreateSelectionProperty(ccu.Namespaces[0].Types[0], property, g_attri);
                    }
                    else
                    {
                        g_property.CreateProperty(ccu.Namespaces[0].Types[0], property, g_attri);
                    }
                }
            }
            return ccu;
        }
    }
}
