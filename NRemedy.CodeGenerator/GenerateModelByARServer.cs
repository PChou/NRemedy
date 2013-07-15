using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;

namespace NRemedy.CodeGenerator
{
    public class GenerateModelByARServer
    {
        private ARLoginContext _context;
        public GenerateModelByARServer(ARLoginContext context)
        {
            _context = context;
        }

        public CodeCompileUnit GenerateModelCCU(string FormName, CSharpModelGeneratorDefaultFactory factory)
        {
            
            ARFieldFilterDelegate filter = factory.CreateARFieldFilterDelegate();

            ARSchema arschema = new ARSchema(_context);
            List<ARField> fields = arschema.GetListFieldWithDetail(FormName, filter);

            CSharpModelGenerator Generator = new CSharpModelGenerator();

            CodeCompileUnit CCU = Generator.GenerateModelCCU(FormName, fields, factory);
            return CCU;
        }
    }
}
 
