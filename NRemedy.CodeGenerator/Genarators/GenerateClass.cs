using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NRemedy
{
    public class GenerateClass : IGenerateClass
    {
        public void Create(CodeNamespace rootNamespace, string className, string formName, IGenerateAttribute attributeGenerator)
        {
            //parameter check
            if (rootNamespace == null) throw new ArgumentNullException("rootNamespace");
            if (string.IsNullOrEmpty(className)) throw new ArgumentNullException("className");
            if (string.IsNullOrEmpty(formName)) throw new ArgumentNullException("formName");

            if (className.Contains(".")) throw new InvalidOperationException("className should not contains '.'");
            if (attributeGenerator == null) throw new ArgumentNullException("attributeGenerator");

            CodeTypeDeclaration classType = new CodeTypeDeclaration(className);
            //指定这是一个Class
            classType.IsClass = true;
            //指定该类为Partial
            classType.IsPartial = true;

            //define MemberAttributes
            classType.Attributes = MemberAttributes.Public;
            //add attribute

            Dictionary<string, object> _attributeProperties = new Dictionary<string, object>();
            _attributeProperties.Add("FormName", formName);
            attributeGenerator.CreateCLassAttribute(classType, typeof(ARFormAttribute), _attributeProperties);

            rootNamespace.Types.Add(classType);
        }
    }
}
