//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using BMC.ARSystem;
//using System.Collections;

//namespace NRemedy
//{
//    /// <summary>
//    /// 实现IARSchema，用于对表单和字段的遍历
//    /// </summary>
//    public class ARSchema : IARSchema
//    {
//        protected ARLoginContext loginContext;

//        public ARSchema(ARLoginContext context, ARServerDefaultFactory factory)
//        {
//            if (context == null) throw new ArgumentNullException("context");
//            if (string.IsNullOrEmpty(context.Server)) throw new ArgumentNullException("context.Server");
//            if (string.IsNullOrEmpty(context.User)) throw new ArgumentNullException("context.User");
//            IARServer server = factory.CreateARServer();
//            context.Login(server);
//            loginContext = context;
//        }

//        public ARSchema(ARLoginContext context) : this(context, new ARServerDefaultFactory())
//        {
//        }

//        public IEnumerable<ARForm> GetListForm()
//        {
            
//            ArrayList list = loginContext.ServerInstance.GetListForm();
//            foreach (string formName in list)
//            {
//                ARForm form = loginContext.ServerInstance.GetForm(formName);
//                yield return form;
//            }
//        }

//        public IEnumerable<ARForm> GetListForm(ARFormFilterDelegate formFilter)
//        {
//            ArrayList list = loginContext.ServerInstance.GetListForm();
//            foreach (string formName in list)
//            {
//                ARForm form = loginContext.ServerInstance.GetForm(formName);
//                if (formFilter != null)
//                {
//                    if (!formFilter(form))
//                        continue;
//                    yield return form;
//                }
//                else
//                    yield return form;
//            }
//        }

//        public IEnumerable<Field> GetListField(string formName)
//        {
//            if (string.IsNullOrEmpty(formName)) throw new ArgumentNullException("formName");
//            ArrayList list = loginContext.ServerInstance.GetListField(formName);
//            foreach (uint fieldID in list)
//            {
//                Field field = loginContext.ServerInstance.GetField(formName, fieldID);
//                yield return field;
//            }
//        }

//        public IEnumerable<Field> GetListField(string formName, ARFieldFilterDelegate fieldFilter)
//        {
//            if (string.IsNullOrEmpty(formName)) throw new ArgumentNullException("formName");
//            ArrayList list = loginContext.ServerInstance.GetListField(formName);
//            foreach (uint fieldID in list)
//            {
//                Field field = loginContext.ServerInstance.GetField(formName, fieldID);
//                if (fieldFilter == null)
//                    yield return field;
//                if (!fieldFilter(field))
//                    continue;
//                yield return field;
//            }
//        }
//    }
//}
