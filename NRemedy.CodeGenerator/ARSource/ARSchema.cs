using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BMC.ARSystem;
using System.Collections;
using ARNative;

namespace NRemedy.CodeGenerator
{
    public class ARSchema : IARSchema
    {
        protected ARLoginContext loginContext;

        public ARSchema(ARLoginContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            loginContext = context;
        }

       

        public List<ARForm> GetListFormWithDetail()
        {
            List<ARForm> result = new List<ARForm>();
            List<string> list = loginContext.ServerInstance.GetListForm();
            foreach (string formName in list)
            {
                ARForm form = loginContext.ServerInstance.GetForm(formName);
                result.Add(form);
            }
            return result;
        }

        public List<ARForm> GetListFormWithDetail(ARFormFilterDelegate formFilter)
        {
            List<ARForm> result = new List<ARForm>();
            List<string> list = loginContext.ServerInstance.GetListForm();
            foreach (string formName in list)
            {
                ARForm form = loginContext.ServerInstance.GetForm(formName);
                if (formFilter == null || formFilter(form))
                {
                    result.Add(form);
                }
            }
            return result;
        }

        public List<ARField> GetListFieldWithDetail(string formName)
        {
            List<ARField> result = new List<ARField>();
            List<UInt32> list = loginContext.ServerInstance.GetListField(formName);
            foreach (uint fieldID in list)
            {
                ARField field = loginContext.ServerInstance.GetField(formName, fieldID);
                result.Add(field);
            }
            return result;
        }

        public List<ARField> GetListFieldWithDetail(string formName, ARFieldFilterDelegate fieldFilter)
        {
            List<ARField> result = new List<ARField>();
            List<UInt32> list = loginContext.ServerInstance.GetListField(formName);
            foreach (uint fieldID in list)
            {
                ARField field = loginContext.ServerInstance.GetField(formName, fieldID);
                if (fieldFilter == null || fieldFilter(field))
                {
                    result.Add(field);
                }
            }
            return result;
        }
    }
}
