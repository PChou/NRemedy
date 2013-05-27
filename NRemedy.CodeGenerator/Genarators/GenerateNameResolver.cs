using System.Text.RegularExpressions;

namespace NRemedy
{
    public class GenerateNameResolver : IGenerateNameResolver
    {
        public virtual string DefaultClassNameResolver(string formName)
        {
            formName = Regex.Replace(formName, @"[^A-Za-z0-9_]", "");
            if (Regex.IsMatch(formName,@"^\d+"))
            {
                formName = "_" + formName;
            }
            return formName;
        }

        public virtual string DefaultFieldNameResolver(string fieldName)
        {
            fieldName = Regex.Replace(fieldName, @"[^A-Za-z0-9_]", "");
            if (Regex.IsMatch(fieldName, @"^\d+"))
            {
                fieldName = "_" + fieldName;
            }
            return "_" + fieldName;
        }

        public virtual string DefaultPropertyNameResolver(string propertyName)
        {
            propertyName = Regex.Replace(propertyName, @"[^A-Za-z0-9_]", "");
            if (Regex.IsMatch(propertyName, @"^\d+"))
            {
                propertyName = "_" + propertyName;
            }
            return propertyName;
        }


        //Custom Selection Type Name for AR Selection Type
        public virtual string SelectionTypeResolver(string fieldName)
        {
            fieldName = Regex.Replace(fieldName, @"[^A-Za-z0-9_]", "");
            if (Regex.IsMatch(fieldName, @"^\d+"))
            {
                fieldName = "_" + fieldName;
            }
            return fieldName + "_Enum";
        }
    }
}
