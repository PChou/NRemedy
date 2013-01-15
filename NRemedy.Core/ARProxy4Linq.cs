////------------------------------------------------------------------
//// System Name:    NRemedy
//// Component:      NRemedy
//// Create by:      Parker Zhou (parkerz@wicresoft.com)
//// Create Date:    2012-08-15
////------------------------------------------------------------------
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using BMC.ARSystem;
//using NRemedy.Linq;
//using System.Reflection;
//using NRemedy;
//using System.Collections;
//using System.Linq.Expressions;

//namespace NRemedy.Linq
//{
//    /// <summary>
//    /// internal class for linq to remedy
//    /// </summary>
//    /// <typeparam name="T">Model type</typeparam>
//    internal class ARProxy4Linq<T> : ARProxy<T>
//    {
//        public ARProxy4Linq(ARLoginContext context, ARServerDefaultFactory factory)
//            : base(context, factory)
//        {
//        }

//        public ARProxy4Linq(ARLoginContext context)
//            : base(context)
//        {
//        }


//        public T GetEntry(
//            string entryID,
//            FieldIdList idList,
//            IModelBinder<T> binder,
//            string formName,
//            Type targetType
//            )
//        {
//            if (String.IsNullOrEmpty(entryID))
//                throw new ArgumentNullException("entry");
//            if (binder == null)
//                throw new ArgumentNullException("binder");
//            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
//                throw new UnLoginException();
//            #region dropped code
//            /*
//            //src : target
//            Dictionary<PropertyInfo, PropertyInfo> propertiesMap = new Dictionary<PropertyInfo, PropertyInfo>();
//            FieldIdList idList = new FieldIdList();
//            Type srcType = typeof(T);
//            foreach (var mm in memeberMaps)
//            {
//                //take src
//                var srcprop = srcType.GetProperty(mm.SourceMemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
//                if (srcprop == null)
//                    throw new MissingMemberException(mm.SourceMemberName + " in " + srcType.ToString());

//                ARFieldAttribute aa = binder.GetARAttributeField(srcprop, ModelBinderAccessLevel.OnlyBind);

//                //take target and assign
//                var tarprop = targetType.GetProperty(mm.TargetMemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
//                if (tarprop == null)
//                    throw new MissingMemberException(mm.TargetMemberName + " in " + targetType.ToString());

//                propertiesMap.Add(srcprop, tarprop);

//                idList.Add(aa.DatabaseID);
//            }

//            T entryModel = binder.Bind(loginContext.ServerInstance.GetEntry(getFormName(typeof(T)), entryID, idList));

//            if (targetType == null)
//                return entryModel;
            


//            object targetModel = Activator.CreateInstance(targetType,null,null);//auto throw exception
//            if (memeberMaps == null || memeberMaps.Count() == 0)
//                return targetModel;
//            foreach (var pm in propertiesMap)
//            {
//                object value = pm.Key.GetValue(entryModel,null);
//                pm.Value.SetValue(targetModel, value,null);
//            }

//            return targetModel;
//             * */
//            #endregion
//            return binder.Bind(loginContext.ServerInstance.GetEntry(formName, entryID, idList));

//        }

//        public IEnumerable GetEntryByQuery(
//            string qualification,
//            //FieldIdList idList,
//            IEnumerable<MemberMap> memeberMaps,
//            Type targetType,
//            LambdaExpression selectExp
//            )
//        {
//            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
//                throw new UnLoginException();
//            //--ready fieldIdList
//            FieldIdList idList = new FieldIdList();
//            ModelBinder4Linq<T> binder = new ModelBinder4Linq<T>();
//            Type srcType = typeof(T);
//            foreach (var mm in memeberMaps)
//            {
//                //take src
//                var srcprop = srcType.GetProperty(mm.SourceMemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
//                if (srcprop == null)
//                    throw new MissingMemberException(mm.SourceMemberName + " in " + srcType.ToString());

//                ARFieldAttribute aa = binder.GetARAttributeField(srcprop, ModelBinderAccessLevel.OnlyBind);

//                idList.Add(aa.DatabaseID);
//            }

//            ArrayList entryIDs = loginContext.ServerInstance.GetListEntry(GetFormName(typeof(T)), qualification);

//            List<T> entries = new List<T>();
//            foreach (var item in entryIDs)
//            {
//                EntryDescription des = (EntryDescription)item;
//                T entry = GetEntry(des.EntryId, idList, binder, GetFormName(typeof(T)), targetType);
//                entries.Add(entry);
//            }

//            Type[] typeArgs = { targetType, typeof(T) };
//            return (IEnumerable)Activator.CreateInstance(
//                    typeof(Enumerator<,>).MakeGenericType(typeArgs),
//                    entries,
//                    selectExp);
//        }

//        public int GetEntryCountByQuery(string qualification)
//        {
//            int count = 0;
//            ArrayList entryIDs;
//            if (string.IsNullOrEmpty(qualification))
//                entryIDs = loginContext.ServerInstance.GetListEntry(GetFormName(typeof(T)), null);
//            else
//                entryIDs = loginContext.ServerInstance.GetListEntry(GetFormName(typeof(T)), qualification);
//            count = entryIDs.Count;

//            return count;
//        }
//    }
//}
