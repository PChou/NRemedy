using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NRemedy.CodeGenerator
{
    public class GenerateClass : IGenerateClass
    {
        public void Create(CodeNamespace rootNamespace, ClassStructure classDefinition, IGenerateAttribute attributeGenerator)
        {
            //parameter check
            if (rootNamespace == null) throw new ArgumentNullException("rootNamespace");
            if (classDefinition == null) throw new ArgumentNullException("classDefinition");
            if (attributeGenerator == null) throw new ArgumentNullException("attributeGenerator");
            if (string.IsNullOrEmpty(classDefinition.ClassName)) throw new ArgumentNullException("classDefinition.ClassName");
            if (classDefinition.ClassName.Contains(".")) throw new InvalidOperationException("className should not contains '.'");

            CodeTypeDeclaration classType = new CodeTypeDeclaration(classDefinition.ClassName);
            //指定这是一个Class
            classType.IsClass = true;
            //指定该类为Partial
            classType.IsPartial = true;

            //define MemberAttributes
            classType.Attributes = MemberAttributes.Public;
            //add attribute
            attributeGenerator.CreateCLassAttribute(classType, classDefinition.ClassAttributeList);

            rootNamespace.Types.Add(classType);
        }
    }
}
