using ARNative;
//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NRemedy
{
    public class ARProxy<T> where T : ARBaseForm
    {
        protected ARLoginContext loginContext;
        public ITypeMetaProvider<T> _metaProvider;

        private static Object lockObject = new Object(); //保证对同一个Form，只有一种操作。

        /// <summary>
        /// Constructor 1 
        /// </summary>
        /// <param name="context">LoginContext</param>
        public ARProxy(ARLoginContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            loginContext = context;
            _metaProvider = new DefaultTypeMetaProvider<T>();
        }

        /// <summary>
        /// Constructor 1 
        /// </summary>
        /// <param name="context">LoginContext</param>
        /// <param name="metaProvider">provider for model metadata</param>
        public ARProxy(ARLoginContext context,ITypeMetaProvider<T> metaProvider)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (metaProvider == null) throw new ArgumentNullException("metaProvider");
            loginContext = context;
            _metaProvider = metaProvider;
        }


        /// <summary>
        /// return IARServer interface
        /// can be used for operate orignal API
        /// </summary>
        /// <returns></returns>
        public IARServer GetARServer()
        {
            return loginContext.ServerInstance;
        }

        public void SetImpersonatedUser(string user)
        {
            loginContext.ServerInstance.SetImpersonatedUser(user);
        }

        #region create entry function

        //formName
        //Model
        //(Properties && Model)
        //                  FieldId
        //                  DataType
        //                  Getter null|not null
        public string CreateEntry(ModelMeteData<T> MetaData)
        {
            if (MetaData == null)
                throw new ArgumentNullException("MetaData");
            if (string.IsNullOrEmpty(MetaData.FormName))
                throw new ArgumentException("MetaData.FormName must provider valid value.");
            if (MetaData.Model == null)
                throw new ArgumentException("MetaData.Model must provider valid value.");
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();

            if (MetaData.Properties == null || MetaData.Properties.Count == 0)
                return null;
            //use MetaData.Properties to build fvl
            List<ARFieldValue> fvl = new List<ARFieldValue>();
            foreach(PropertyAndField<T> pf in MetaData.Properties)
            {
                ARFieldValue fv = new ARFieldValue();
                fv.FieldId = pf.DatabaseId;
                fv.DataType = (ARDataType)pf.DatabaseType;
                fv.Value = pf.GetValueC(MetaData.Model);
                fvl.Add(fv);
            }

            return loginContext.ServerInstance.CreateEntry(MetaData.FormName, fvl);
 
        }

        //public string CreateEntry(List<ARFieldValue> FieldValueList)
        //{
        //    string formName = _metaProvider.GetFormNameFromModelType(typeof(T));
        //    ModelMeteData<T> metaData = new ModelMeteData<T>();
        //    metaData.FieldValeList = FieldValueList;
        //    metaData.FormName = formName;
        //    return CreateEntry(metaData);
        //}

        public string CreateEntry(T entry,PropertyFilterDelegate2 filter)
        {
            if (entry == null) throw new ArgumentNullException("entry");
            ModelMeteData<T> metaData = new ModelMeteData<T>();
            metaData.FormName = _metaProvider.GetFormNameFromModelType();
            metaData.Model = entry;
            var props = _metaProvider.GetPropertyInfoes(
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
                filter
                );
            metaData.Properties = new List<PropertyAndField<T>>();

            foreach (var p in props){
                if ((p.AccessLevel & ModelBinderAccessLevel.OnlyUnBind) == ModelBinderAccessLevel.OnlyUnBind){
                    metaData.Properties.Add(p);
                }
            }

            return CreateEntry(metaData);

        }

        public string CreateEntry(T entry)
        {
            return CreateEntry(entry, null);
        }

        #endregion

        #region delete entry function

        //formName
        //entryid
        public void DeleteEntry(ModelMeteData<T> MetaData)
        {
            if (MetaData == null)
                throw new ArgumentNullException("MetaData");
            if (string.IsNullOrEmpty(MetaData.EntryId))
                throw new ArgumentException("MetaData.EntryId must provider valid value.");
            if (string.IsNullOrEmpty(MetaData.FormName))
                throw new ArgumentException("MetaData.FormName must provider valid value.");
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();
            loginContext.ServerInstance.DeleteEntry(MetaData.FormName, MetaData.EntryId, 0);
        }

        /// <summary>
        /// delete entry by entryid
        /// </summary>
        /// <param name="EntryId">entry id for the model</param>
        public void DeleteEntry(string EntryId)
        {
            ModelMeteData<T> meta = new ModelMeteData<T>();
            meta.EntryId = EntryId;
            meta.FormName = _metaProvider.GetFormNameFromModelType();
            DeleteEntry(meta);
        }

        /// <summary>
        /// Delete AR Entry by single model
        /// </summary>
        /// <param name="entry">entry model to be delete, must have not null entryid property</param>
        public void DeleteEntry(T entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            var entryIdProp = _metaProvider.GetEntryIdPropertyInfo();
            if (entryIdProp == null)
                throw new CustomAttributeFormatException("Can not find EntryId's PropertyInfo.");
            ModelMeteData<T> meta = new ModelMeteData<T>();
            meta.EntryId = (string)entryIdProp.GetValue(entry);
            meta.FormName = _metaProvider.GetFormNameFromModelType();
            DeleteEntry(meta);
        }


        public void DeleteEntryList(string qualification)
        {
            ModelMeteData<T> meta = new ModelMeteData<T>();
            meta.FormName = _metaProvider.GetFormNameFromModelType();
            int total = -1;
            List<AREntry> entries = loginContext.ServerInstance.GetEntryList(meta.FormName, qualification, null, 0, null,ref total, null);
            foreach (var entry in entries)
            {
                string entryid = string.Join("|", entry.EntryIds.ToArray());
                meta.EntryId = entryid;
                DeleteEntry(meta);
            }
        }


        #endregion

        #region set entry function

        //formname
        //entryid
        //(Properties && Model)
        //                  FieldId
        //                  DataType
        //                  Getter null|not null
        public void SetEntry(ModelMeteData<T> MetaData)
        {
            if (MetaData == null)
                throw new ArgumentNullException("MetaData");
            if (string.IsNullOrEmpty(MetaData.EntryId))
                throw new ArgumentException("MetaData.EntryId must provider valid value.");
            if (string.IsNullOrEmpty(MetaData.FormName))
                throw new ArgumentException("MetaData.FormName must provider valid value.");
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();

            if (MetaData.Properties == null || MetaData.Properties.Count == 0)
                return;
            //use MetaData.Properties to build fvl
            List<ARFieldValue> fvl = new List<ARFieldValue>();
            foreach (PropertyAndField<T> pf in MetaData.Properties)
            {
                ARFieldValue fv = new ARFieldValue();
                fv.FieldId = pf.DatabaseId;
                fv.DataType = (ARDataType)pf.DatabaseType;
                fv.Value = pf.GetValueC(MetaData.Model);
                fvl.Add(fv);
            }

            loginContext.ServerInstance.SetEntry(MetaData.FormName, MetaData.EntryId, fvl);

        }

        public void SetEntry(string EntryId, List<ARFieldValue> FieldValueList)
        {
            string formName = _metaProvider.GetFormNameFromModelType();

            loginContext.ServerInstance.SetEntry(formName, EntryId, FieldValueList);

            //ModelMeteData<T> metaData = new ModelMeteData<T>();
            ////metaData.FieldValeList = FieldValueList;

            //var properties = _metaProvider.GetPropertyInfoes(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null);
            //List<PropertyAndField<T>> props = new List<PropertyAndField<T>>();
            //foreach (var p in properties)
            //{
            //    if (FieldValueList.Find(delegate(ARFieldValue arfv) { return arfv.FieldId == p.DatabaseId; }) != null)
            //        props.Add(p);
            //}

            //metaData.Properties = props;
            //metaData.FormName = formName;
            //metaData.EntryId = EntryId;
            //SetEntry(metaData);
        }

        public void SetEntry(T entry, PropertyFilterDelegate filter)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            ModelMeteData<T> meta = new ModelMeteData<T>();
            meta.FormName = _metaProvider.GetFormNameFromModelType();
            var entryIdProp = _metaProvider.GetEntryIdPropertyInfo();
            if (entryIdProp == null)
                throw new CustomAttributeFormatException("Can not find EntryId's PropertyInfo.");

            meta.EntryId = (string)entryIdProp.GetValue(entry);
            meta.Model = entry;

            var props = _metaProvider.GetPropertyInfoes(entry as ARBaseForm,
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
                filter
                );

            meta.Properties = new List<PropertyAndField<T>>();

            foreach (var p in props)
            {
                if ((p.AccessLevel & ModelBinderAccessLevel.OnlyUnBind) == ModelBinderAccessLevel.OnlyUnBind)
                {
                    meta.Properties.Add(p);
                }
            }

            SetEntry(meta);
        }

        public void SetEntry(T entry)
        {
            SetEntry(entry, delegate(ARBaseForm model, PropertyInfo pi) 
                {

                    if (model == null)
                        throw new ArgumentNullException("model", "model is probably not inherit from ARBaseForm");
                    if (pi == null)
                        throw new ArgumentNullException("pi");

                    bool result = false;

                    if (model.ChangedProperties.ContainsKey(pi.Name))
                        result = model.ChangedProperties[pi.Name];
                    else
                        result = false;

                    return result;
                }
            );
        }

        public void SetEntry(T entry, IEnumerable<string> Properties)
        {
            List<string> props = new List<string>(Properties);
            SetEntry(entry, delegate(ARBaseForm model, PropertyInfo pi)
                {
                    if (pi == null)
                        throw new ArgumentNullException("pi");
                    return props.Contains(pi.Name);
                }
            );
        }

        public void SetEntryList(string qualification, List<ARFieldValue> FieldValueList)
        {
            ModelMeteData<T> meta = new ModelMeteData<T>();
            meta.FormName = _metaProvider.GetFormNameFromModelType();
            int total = -1;
            List<AREntry> entries = loginContext.ServerInstance.GetEntryList(meta.FormName, qualification, null, 0, null, ref total, null);

            var properties = _metaProvider.GetPropertyInfoes(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null);
            List<PropertyAndField<T>> props = new List<PropertyAndField<T>>();
            foreach (var p in properties)
            {
                if (FieldValueList.Find(delegate(ARFieldValue arfv) { return arfv.FieldId == p.DatabaseId; }) != null)
                    props.Add(p);
            }

            meta.Properties = props;

            foreach (var entry in entries)
            {
                string entryid = string.Join("|", entry.EntryIds.ToArray());
                meta.EntryId = entryid;
                SetEntry(meta);
            }
 
        }

        public void SetEntryList(string qualification,T entry, IEnumerable<string> Properties)
        {
            string formName = _metaProvider.GetFormNameFromModelType();
            int total = -1;
            List<AREntry> entries = loginContext.ServerInstance.GetEntryList(formName, qualification, null, 0, null, ref total, null);
            var entryIdProp = _metaProvider.GetEntryIdPropertyInfo();
            foreach (var e in entries)
            {
                string entryid = string.Join("|", e.EntryIds.ToArray());
                entryIdProp.SetValue(entry, entryid);
                SetEntry(entry, Properties);
            }
        }

        #endregion


        #region get entry function

        //formname
        //entryid
        //Properties
        //              FieldId
        //              DataType
        //              Setter null|not null
        public T GetEntry(ModelMeteData<T> MetaData)
        {
            if (MetaData == null)
                throw new ArgumentNullException("MetaData");
            if (string.IsNullOrEmpty(MetaData.EntryId))
                throw new ArgumentException("MetaData.EntryId must provider valid value.");
            if (string.IsNullOrEmpty(MetaData.FormName))
                throw new ArgumentException("MetaData.FormName must provider valid value.");
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();
            if (MetaData.Properties == null || MetaData.Properties.Count == 0)
                throw new ArgumentException("MetaData.Properties must provider valid value.");

            List<ARFieldValue> raw;
            Dictionary<uint,PropertyAndField<T>> mapps = new Dictionary<uint,PropertyAndField<T>>();

            List<uint> fil = new List<uint>();
                
            foreach (var prop in MetaData.Properties){
                fil.Add(prop.DatabaseId);
                mapps.Add(prop.DatabaseId,prop);
            }
            raw = loginContext.ServerInstance.GetEntry(MetaData.FormName, MetaData.EntryId,fil);


            if (raw == null)
                return null;

            T model = Activator.CreateInstance<T>();

            foreach (var arfv in raw){
                PropertyAndField<T> t_p = mapps[arfv.FieldId];
                t_p.SetValueC(model, arfv.Value);
            }
            return model;
        }

        public T GetEntry(string EntryId, List<UInt32> FieldIdList)
        {
            string formName = _metaProvider.GetFormNameFromModelType();
            ModelMeteData<T> metaData = new ModelMeteData<T>();

            var properties = _metaProvider.GetPropertyInfoes(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null);
            List<PropertyAndField<T>> props = new List<PropertyAndField<T>>();
            foreach (var p in properties)
            {
                if (FieldIdList.Find(delegate(UInt32 arfv) 
                { 
                    return arfv == p.DatabaseId;
                }) 
                != 0)
                    props.Add(p);
            }

            metaData.Properties = props;
            metaData.FormName = formName;
            metaData.EntryId = EntryId;
            return GetEntry(metaData);
        }

        public T GetEntry(string EntryId, PropertyFilterDelegate2 filter)
        {
            string formName = _metaProvider.GetFormNameFromModelType();
            ModelMeteData<T> metaData = new ModelMeteData<T>();
            //metaData.FieldIdList = FieldIdList;
            metaData.FormName = formName;
            metaData.EntryId = EntryId;
            var props = _metaProvider.GetPropertyInfoes(
                BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance,
                filter
                );
            metaData.Properties = new List<PropertyAndField<T>>();
            foreach (var p in props)
            {
                if((p.AccessLevel & ModelBinderAccessLevel.OnlyBind) == ModelBinderAccessLevel.OnlyBind)
                {
                    metaData.Properties.Add(p);
                }

            }

            return GetEntry(metaData);
        }

        public T GetEntry(string EntryId, IEnumerable<string> Properties)
        {
            List<string> valid = new List<string>(Properties);

            return GetEntry(EntryId, delegate(PropertyInfo pi) 
                {
                    if (pi == null)
                        throw new ArgumentNullException("pi");
                    return valid.Contains(pi.Name);

                }
            );
        }

        public T GetEntry(string EntryId)
        {
            return GetEntry(EntryId, delegate(PropertyInfo pi)
            {
                return true;
            }
            );
        }

        //formname
        //Properties
        //              FieldId
        //              DataType
        //              Setter null|not null
        public IList<T> GetEntryList(
            uint StartIndex,
            string qualification,
            ModelMeteData<T> MetaData,
            uint? RetrieveCount,
            TotalMatch totalMatch,
            List<ARSortInfo> sortInfo
            )
        {
            if (MetaData == null)
                throw new ArgumentNullException("MetaData");
            if (string.IsNullOrEmpty(MetaData.FormName))
                throw new ArgumentException("MetaData.FormName must provider valid value.");
            if(MetaData.Properties == null && MetaData.Properties.Count == 0)
            {
                throw new ArgumentException("MetaData.Properties must provider valid value.");
            }
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();
            int total = 0;
            if(totalMatch == null)
                total = -1;
            List<AREntry> raw_entries = null;
            Dictionary<uint,PropertyAndField<T>> mapps = new Dictionary<uint,PropertyAndField<T>>();

            List<uint> fil = new List<uint>();
                
            foreach (var prop in MetaData.Properties){
                fil.Add(prop.DatabaseId);
                mapps.Add(prop.DatabaseId,prop);
            }
            raw_entries = loginContext.ServerInstance.GetEntryList
                (MetaData.FormName,qualification,fil,StartIndex,RetrieveCount,ref total,sortInfo);

            if (raw_entries == null)
                return null;
            List<T> listT = new List<T>();

            foreach(var raw in raw_entries)
            {
                //TOTEST:join form condition may have some issue
                T model = Activator.CreateInstance<T>();
                //string entryid = string.Join("|", raw.EntryIds.ToArray());
                //mapps[1].SetValue(model,entryid);
                foreach(var fv in raw.FieldValues){
                    mapps[fv.FieldId].SetValueC(model,fv.Value);
                }
                listT.Add(model);
            }
            if (totalMatch != null)
                totalMatch.Value = total;
            return listT;
        }

        public IList<T> GetEntryList(
            string qualification,
            List<uint> FieldIdList,
            uint StartIndex,
            uint? RetrieveCount,
            TotalMatch totalMatch,
            List<ARSortInfo> sortInfo
            )
        {
            ModelMeteData<T> meta = new ModelMeteData<T>();
            meta.FormName = _metaProvider.GetFormNameFromModelType();
            var properties = _metaProvider.GetPropertyInfoes(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null);
            List<PropertyAndField<T>> props = new List<PropertyAndField<T>>();
            foreach (var p in properties)
            {
                if (FieldIdList.Find(delegate(UInt32 arfv) { return arfv == p.DatabaseId; }) != 0)
                    props.Add(p);
            }
            meta.Properties = props;

            return GetEntryList(StartIndex,qualification, meta, RetrieveCount, totalMatch, sortInfo);
        }

        public IList<T> GetEntryList(
            string qualification,
            uint StartIndex,
            PropertyFilterDelegate2 filter,
            uint? RetrieveCount,
            TotalMatch totalMatch,
            List<ARSortInfo> sortInfo
            )
        {
            ModelMeteData<T> metaData = new ModelMeteData<T>();
            metaData.FormName = _metaProvider.GetFormNameFromModelType();
            var props = _metaProvider.GetPropertyInfoes(
                BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance,
                filter
                );
            metaData.Properties = new List<PropertyAndField<T>>();
            foreach (var p in props){
                if ((p.AccessLevel & ModelBinderAccessLevel.OnlyBind) == ModelBinderAccessLevel.OnlyBind)
                {
                    metaData.Properties.Add(p);
                }

            }

            return GetEntryList(StartIndex,qualification, metaData, RetrieveCount,totalMatch, sortInfo);

        }

        public IList<T> GetEntryList(
            string qualification,
            IEnumerable<string> Properties,
            uint? RetrieveCount,
            uint StartIndex,
            TotalMatch totalMatch,
            List<ARSortInfo> sortInfo
            )
        {
            List<string> valid = null;
            if (Properties != null)
            {
                valid = new List<string>(Properties);
            }
            else
            {
                valid = new List<string>();
            }
            
            return GetEntryList(
                    qualification,
                    StartIndex,
                    delegate(PropertyInfo pi)
                        {
                            if (pi == null)
                                throw new ArgumentNullException("pi");
                            return valid.Contains(pi.Name);
                        },
                    RetrieveCount,
                    totalMatch,
                    sortInfo
                );
        }

        #endregion

        /// <summary>
        /// GetListEntryStatictisc function
        /// the return value is the model list, but have the two situation:
        /// 1.GroupbyFieldIdList is null or count is 0
        ///     the return list must have one entry,no field will be bind into the entry , the result is stored in base class(ARBaseForm)'s Statictis
        /// 2.
        ///     the return list may have more than one entry, group by field will be bind to each entry, and the result is stored in base class(ARBaseForm)'s Statictis
        /// note : the result is object , but mostly the actual type is double,don't forget to Convert
        /// </summary>
        /// <param name="SchemaName"></param>
        /// <param name="Qulification"></param>
        /// <param name="ARStat"></param>
        /// <param name="TargetFieldId"></param>
        /// <param name="Properties"></param>
        /// <returns></returns>
        public IList<T> GetListEntryStatictisc(
            String Qulification,
            Nullable<UInt32> TargetFieldId,
            IEnumerable<string> Properties,
            ARStatictisc ARStat
            )
        {
            List<string> valid = new List<string>(Properties);

            return GetListEntryStatictisc(Qulification,  TargetFieldId,ARStat,delegate(PropertyInfo pi)
            {
                if (pi == null)
                    throw new ArgumentNullException("pi");
                return valid.Contains(pi.Name);

            }
            );
 
        }


        protected IList<T> GetListEntryStatictisc(
            String Qulification,
            Nullable<UInt32> TargetFieldId,
            ARStatictisc ARStat,
            PropertyFilterDelegate2 filter
            )
        {
            ModelMeteData<T> metaData = new ModelMeteData<T>();
            metaData.FormName = _metaProvider.GetFormNameFromModelType();
            var props = _metaProvider.GetPropertyInfoes(
                BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance,
                filter
                );
            metaData.Properties = new List<PropertyAndField<T>>();
            foreach (var p in props)
            {
                if ((p.AccessLevel & ModelBinderAccessLevel.OnlyBind) == ModelBinderAccessLevel.OnlyBind)
                {
                    metaData.Properties.Add(p);
                }

            }

            return GetListEntryStatictisc(ARStat,Qulification,  TargetFieldId,metaData);
 
        }

        /// <summary>
        /// GetListEntryStatictisc function
        /// the return value is the model list, but have the two situation:
        /// 1.GroupbyFieldIdList is null or count is 0
        ///     the return list must have one entry,no field will be bind into the entry , the result is stored in base class(ARBaseForm)'s Statictis
        /// 2.
        ///     the return list may have more than one entry, group by field will be bind to each entry, and the result is stored in base class(ARBaseForm)'s Statictis
        /// note : the result is object , but mostly the actual type is double,don't forget to Convert
        /// </summary>
        /// <param name="SchemaName"></param>
        /// <param name="Qulification"></param>
        /// <param name="ARStat"></param>
        /// <param name="TargetFieldId"></param>
        /// <param name="GroupbyFieldIdList"></param>
        /// <returns></returns>
        public IList<T> GetListEntryStatictisc(
            String Qulification,
            ARStatictisc ARStat,
            Nullable<UInt32> TargetFieldId,
            List<UInt32> GroupbyFieldIdList
            )
        {
            string formName = _metaProvider.GetFormNameFromModelType();
            ModelMeteData<T> metaData = new ModelMeteData<T>();
            if (GroupbyFieldIdList != null)
            {
                var properties = _metaProvider.GetPropertyInfoes(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null);
                List<PropertyAndField<T>> props = new List<PropertyAndField<T>>();
                foreach (var p in properties)
                {
                    if (GroupbyFieldIdList.Find(delegate(UInt32 arfv) { return arfv == p.DatabaseId; }) != 0)
                        props.Add(p);
                }

                metaData.Properties = props;
            }
            metaData.FormName = formName;
            return GetListEntryStatictisc(ARStat,Qulification,TargetFieldId, metaData);

            ////tagetfiledid must not null if ARStat is not count
            //if (TargetFieldId == null && ARStat != ARStatictisc.STAT_OP_COUNT)
            //    throw new InvalidOperationException("TargetFieldId must not null if ARStat is not COUNT");
            //string formName = _metaProvider.GetFormNameFromModelType();
            //List<ARGroupByStatictisc> result = loginContext.ServerInstance.GetEntryListStatictisc(
            //    formName,
            //    Qulification,
            //    ARStat,
            //    TargetFieldId,
            //    GroupbyFieldIdList);

            //List<T> models = new List<T>();

            ////if GroupbyFieldIdList is null, the result must only one row
            ////tagetfiledid can be ignore
            //if (GroupbyFieldIdList == null || GroupbyFieldIdList.Count == 0)
            //{
            //    T model = Activator.CreateInstance<T>();
            //    if (!(model is ARBaseForm))
            //        throw new InvalidCastException("T must be inherit from ARBaseForm so that Statictisc operation can be cast.");
            //    (model as ARBaseForm).Statictisc = result[0].Statictisc;
            //    models.Add(model);
            //}
            ////else GroupbyFieldIdList is not null,the result may have rows more than 1
            ////and targetfieldid can be ignore
            //else
            //{
            //    //store the arid and artype map
            //    Dictionary<uint, ARType> idmaptype = new Dictionary<uint, ARType>();

            //    foreach (ARGroupByStatictisc g in result)
            //    {
            //        T model = Activator.CreateInstance<T>();
            //        List<ARFieldValue> fv = new List<ARFieldValue>();
            //        for (int i = 0; i < g.GroupByValues.Count; i++)
            //        {
            //            if (!idmaptype.ContainsKey(GroupbyFieldIdList[i]))
            //                idmaptype.Add(GroupbyFieldIdList[i], _metaProvider.GetARTypeFromFieldId(typeof(T),GroupbyFieldIdList[i]));
            //            fv.Add(new ARFieldValue(GroupbyFieldIdList[i], g.GroupByValues[i], (ARDataType)idmaptype[GroupbyFieldIdList[i]]));
            //        }
            //        //bind model property from groupvalues
            //        model = binder.Bind(fv);
            //        //add statictisc result to the ARBaseForm
            //        if (!(model is ARBaseForm))
            //            throw new InvalidCastException("T must be inherit from ARBaseForm so that Statictisc operation can be cast.");
            //        (model as ARBaseForm).Statictisc = g.Statictisc;
            //        models.Add(model);
            //    }
            //}


            //return models;
        }

        //formname
        //Properties
        //              FieldId
        //              DataType
        //              Setter null|not null
        public IList<T> GetListEntryStatictisc(
            ARStatictisc ARStat,
            String Qulification,
            Nullable<UInt32> TargetFieldId,
            ModelMeteData<T> MetaData
           )
        {
            //tagetfiledid must not null if ARStat is not count
            if (TargetFieldId == null && ARStat != ARStatictisc.STAT_OP_COUNT)
                throw new InvalidOperationException("TargetFieldId must not null if ARStat is not COUNT");

            List<ARGroupByStatictisc> raw_entries = null;
            List<T> models = new List<T>();
            Dictionary<uint, PropertyAndField<T>> mapps = new Dictionary<uint, PropertyAndField<T>>();
            
            if (MetaData.Properties == null || MetaData.Properties.Count == 0)
            {
                raw_entries = loginContext.ServerInstance.GetEntryListStatictisc(MetaData.FormName, Qulification, ARStat, TargetFieldId, null);
                T model = Activator.CreateInstance<T>();
                if (!(model is ARBaseForm))
                    throw new InvalidCastException("T must be inherit from ARBaseForm so that Statictisc operation can be cast.");
                model.Statictisc = raw_entries[0].Statictisc;
                models.Add(model);
                return models;
            }


            List<uint> fil = new List<uint>();

            foreach (var prop in MetaData.Properties)
            {
                fil.Add(prop.DatabaseId);
                mapps.Add(prop.DatabaseId, prop);
            }
            raw_entries = loginContext.ServerInstance.GetEntryListStatictisc(MetaData.FormName, Qulification, ARStat, TargetFieldId, fil);

            if (raw_entries == null)
                return null;

            foreach (var raw in raw_entries)
            {
                T model = Activator.CreateInstance<T>();
                var enumer1 = mapps.GetEnumerator();
                var enumer2 = raw.GroupByValues.GetEnumerator();
                while (enumer1.MoveNext() && enumer2.MoveNext())
                {
                    enumer1.Current.Value.SetValueC(model, enumer2.Current);
                    model.Statictisc = raw.Statictisc;
                } 
                models.Add(model);
            }


            return models;
        }

    }

}
