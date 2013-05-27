using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy
{
    public class CSharpModelGeneratorDefaultFactory
    {
        public virtual IGenerateNameResolver CreateGenerateNameResolver()
        {
            return new GenerateNameResolver();
        }

        public virtual IGenerateRootNamespace CreateGenerateRootNamespace()
        {
            return new GenerateRootNamespace();
        }

        public virtual IGenerateImportNamespace CreateGenerateImportNamespace()
        {
            return new GenerateImportNamespace();
        }

        public virtual IGenerateAttribute CreateGenerateAttribute()
        {
            return new GenerateAttribute();
        }

        public virtual IGenerateClass CreateGenerateClass()
        {
            return new GenerateClass();
        }

        public virtual IGenerateField CreateGenerateField()
        {
            return new GenerateField();
        }

        public virtual IGenerateProperty CreateGenerateProperty()
        {
            return new GenerateProperty();
        }

        public virtual IARSchema CreateARSchema(ARLoginContext context)
        {
            return new ARSchema(context);
        }

        public virtual ARFieldFilterDelegate CreateARFieldFilterDelegate()
        {
            return GetARFieldFilterDelegate.getARFieldFilterDelegate();
        }
    }
}
