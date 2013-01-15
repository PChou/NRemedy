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
    /// <summary>
    /// ARProxy
    /// </summary>
    /// <example>
    ///string ARServer = ConfigurationManager.AppSettings["ARServer"];
    ///string ARUid = ConfigurationManager.AppSettings["ARUid"];
    ///string ARPwd = ConfigurationManager.AppSettings["ARPwd"];
    ///ARLoginContext context = new ARLoginContext(ARServer, ARUid, ARPwd);
    ///ARProxy&lt;Rack&gt;  rack = new ARProxy&lt;Rack&gt; (context);
    ///string querystr = string.Format("\'InstanceId\'=\"{0}\"", instanceId.Text);
    ///Rack ra = rack.GetEntryByQuery(querystr)[0];
    ///ra.Loaction="rack_Location"
    ///rack.SetEntry(ra); 
    /// </example>
    /// <typeparam name="T">Type inherit from ARBaseForm</typeparam>
    public class ARProxy<T>
    {
        protected ARLoginContext loginContext;
        private static Object lockObject = new Object(); //保证对同一个Form，只有一种操作。

        /// <summary>
        /// Constructor 1 
        /// </summary>
        /// <!--William Wang-->
        /// <param name="context">LoginContext</param>
        /// <param name="factory">Server Factory</param>
        public ARProxy(ARLoginContext context, IARServerFactory factory)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (string.IsNullOrEmpty(context.Server)) throw new ArgumentNullException("context.Server");
            if (string.IsNullOrEmpty(context.User)) throw new ArgumentNullException("context.User");
            IARServer server = factory.CreateARServer();
            context.Login(server);
            loginContext = context;
        }

        /// <summary>
        /// Constructor 2
        /// </summary>
        /// <!--William Wang-->
        /// <param name="context">LoginContext</param>
        public ARProxy(ARLoginContext context)
            : this(context, new ARServerDefaultFactory())
        {
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
        /// <summary>
        /// Create AR Entry by model using default modelbinder 
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entry">model</param>
        public string CreateEntry(T entry)
        {
            return CreateEntry(entry, DefaultFactory.CreateModelBinder<T>());
        }

        /// <summary>
        /// Create AR Entry by model, and indicate modelbinder、FormNameUtil
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entry">model</param>
        /// <param name="binder">ModelBinder</param>
        /// <param name="getFormName">getFormName Delegate</param>
        public string CreateEntry(T entry, IModelBinder<T> binder)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (binder == null)
                throw new ArgumentNullException("binder");
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();
            string formName = GetFormName(typeof(T));
            lock (lockObject)
            {
                return loginContext.ServerInstance.CreateEntry(formName, binder.UnBind(entry));
            }
        }



        /// <summary>
        /// Create list entry
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entries">List Model</param>
        /// <param name="binder">ModelBinder</param>
        /// <param name="isEnableTransaction">is enable transaction</param>
        //public void CreateListEntry(IList<T> entries, IModelBinder<T> binder,  bool isEnableTransaction = false)
        //{
        //    if (entries == null || entries.Count == 0)
        //        throw new ArgumentNullException("entries");
        //    if (binder == null)
        //        throw new ArgumentNullException("binder");
        //    if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
        //        throw new UnLoginException();
        //    lock (lockObject)
        //    {
        //        try
        //        {
        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.BeginBulkEntryTransaction();
        //            }
        //            foreach (T item in entries)
        //            {
        //                CreateEntry(item, binder, GetFormName, 2);
        //            }
        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.SendBulkEntryTransaction();
        //            }
        //        }
        //        catch
        //        {
        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.CancelBulkEntryTransaction();
        //            }
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// Create list entry
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entries">List Model</param>
        /// <param name="isEnableTransaction">is enable transaction</param>
        //public void CreateListEntry(IList<T> entries, bool isEnableTransaction = false)
        //{
        //    if (entries == null || entries.Count == 0)
        //        throw new ArgumentNullException("entries");
        //    if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
        //        throw new UnLoginException();
        //    CreateListEntry(entries, DefaultFactory.CreateModelBinder<T>(), GetFormName, isEnableTransaction);
        //}

        /// <summary>
        /// delete entry by entryid
        /// </summary>
        /// <param name="EntryId"></param>
        public void DeleteEntry(string formName,string EntryId)
        {
            if (string.IsNullOrEmpty(formName))
                throw new Exception("formName must not be null or empty.");
            if (string.IsNullOrEmpty(EntryId))
                return;
            lock (lockObject)
            {
                loginContext.ServerInstance.DeleteEntry(formName, EntryId, 0);
            }
        }


        /// <summary>
        /// Delete AR Entry by model, and indicate modelbinder、FormNameUtil
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entry">model</param>
        public void DeleteEntry(T entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();
            string formName = GetFormName(typeof(T));
            DeleteEntry(formName, GetEntryId(entry));
        }


        /// <summary>
        /// Delete list entry
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entries">List Model</param>
        /// <param name="getFormName">getFormName</param>
        /// <param name="isEnableTransaction">is enable transaction</param>
        //public void DeleteListEntry(IList<T> entries, ARGetFormNameDelegate getFormName, bool isEnableTransaction = false)
        //{
        //    if (entries == null || entries.Count == 0)
        //        throw new ArgumentNullException("entries");
        //    if (getFormName == null)
        //        throw new ArgumentNullException("getFormName");
        //    if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
        //        throw new UnLoginException();
        //    lock (lockObject)
        //    {
        //        try
        //        {

        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.BeginBulkEntryTransaction();
        //            }
        //            foreach (T item in entries)
        //            {
        //                DeleteEntry(item, GetFormName, 3);
        //            }
        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.SendBulkEntryTransaction();
        //            }
        //        }
        //        catch
        //        {
        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.CancelBulkEntryTransaction();
        //            }
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// Delete list entry
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entries">List Model</param>
        /// <param name="isEnableTransaction">is enable transaction</param>
        //public void DeleteListEntry(IList<T> entries, bool isEnableTransaction = false)
        //{
        //    if (entries == null || entries.Count == 0)
        //        throw new ArgumentNullException("entries");
        //    if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
        //        throw new UnLoginException();
        //    DeleteListEntry(entries, GetFormName, isEnableTransaction);
        //}

        /// <summary>
        /// Delete entris by query
        /// </summary>
        /// <!--William Wang-->
        /// <param name="qualification"> for example: "\'cChr_ProcessType\' LIKE \"%geGen%\" AND \'cInt_ApproveOrder\'=1004"  </param>
        /// <param name="getFormName">getFormName</param>
        /// <param name="isEnableTransaction">is enable transaction</param>
        /// <param name="useLocale">whether to use locale while getting the matched entry list </param>
        //public void DeleteEntryByQuery(string qualification, ARGetFormNameDelegate getFormName,
        //    bool isEnableTransaction = false, bool useLocale = false)
        //{
        //    if (String.IsNullOrEmpty(qualification))
        //        throw new ArgumentNullException("qualification");
        //    if (getFormName == null)
        //        throw new ArgumentNullException("getFormName");
        //    if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
        //        throw new UnLoginException();
        //    lock (lockObject)
        //    {
        //        try
        //        {

        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.BeginBulkEntryTransaction();
        //            }

        //            foreach (EntryDescription item in loginContext.ServerInstance.GetListEntry(getFormName(typeof(T)), qualification))
        //            {
        //                EntryDescription des = (EntryDescription)item;
        //                loginContext.ServerInstance.DeleteEntry(getFormName(typeof(T)), des.EntryId);
        //            }

        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.SendBulkEntryTransaction();
        //            }
        //        }
        //        catch
        //        {
        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.CancelBulkEntryTransaction();
        //            }
        //            throw;
        //        }
        //    }

        //}


        /// <summary>
        /// Delete entris by query
        /// </summary>
        /// <!--William Wang-->
        /// <param name="qualification"> for example: "\'cChr_ProcessType\' LIKE \"%geGen%\" AND \'cInt_ApproveOrder\'=1004"  </param>
        /// <param name="isEnableTransaction">is enable transaction</param>
        /// <param name="useLocale">whether to use locale while getting the matched entry list </param>
        //public void DeleteEntryByQuery(string qualification, bool isEnableTransaction = false, bool useLocale = false)
        //{
        //    DeleteEntryByQuery(qualification, GetFormName, isEnableTransaction, useLocale);
        //}

        /// <summary>
        /// Set AR Entry by model
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entry">model</param>
        /// <param name="binder">ModelBinder</param>
        public void SetEntry(T entry, IModelBinder<T> binder)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (binder == null)
                throw new ArgumentNullException("binder");
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();
            string formName = GetFormName(typeof(T));
            lock (lockObject)
            {
                loginContext.ServerInstance.SetEntry(formName, GetEntryId(entry), binder.UnBindForUpdate(entry));
            }
        }

        /// <summary>
        /// Set AR Entry by model using default modelbinder、default getFormName
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entry">model</param>
        public void SetEntry(T entry)
        {
            SetEntry(entry, DefaultFactory.CreateModelBinder<T>());
        }

        /// <summary>
        /// Set list entry
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entries">List Model</param>
        /// <param name="binder">ModelBinder</param>
        /// <param name="getFormName">getFormName</param>
        /// <param name="isEnableTransaction">is enable transaction</param>
        //public void SetListEntry(IList<T> entries, IModelBinder<T> binder, ARGetFormNameDelegate getFormName, bool isEnableTransaction = false)
        //{
        //    if (entries == null || entries.Count == 0)
        //        throw new ArgumentNullException("entries");
        //    if (binder == null)
        //        throw new ArgumentNullException("binder");
        //    if (getFormName == null)
        //        throw new ArgumentNullException("getFormName");
        //    if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
        //        throw new UnLoginException();
        //    lock (lockObject)
        //    {
        //        try
        //        {

        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.BeginBulkEntryTransaction();
        //            }
        //            foreach (T item in entries)
        //            {
        //                SetEntry(item, binder, GetFormName, 1);
        //            }
        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.SendBulkEntryTransaction();
        //            }
        //        }
        //        catch
        //        {
        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.CancelBulkEntryTransaction();
        //            }
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// Set list entry
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entries">List Model</param>
        /// <param name="isEnableTransaction">is enable transaction</param>
        //public void SetListEntry(IList<T> entries, bool isEnableTransaction = false)
        //{
        //    if (entries == null || entries.Count == 0)
        //        throw new ArgumentNullException("entries");
        //    if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
        //        throw new UnLoginException();
        //    SetListEntry(entries, DefaultFactory.CreateModelBinder<T>(), GetFormName);
        //}

        /// <summary>
        /// Set entris by query
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entry"> entry</param>
        /// <param name="qualification"> for example: "\'cChr_ProcessType\' LIKE \"%geGen%\" AND \'cInt_ApproveOrder\'=1004"  </param>
        /// <param name="binder">binder</param>
        /// <param name="getFormName">ARGetFormNameDelegate</param>
        /// <param name="isEnableTransaction">isEnableTransaction</param>
        /// <param name="noMatchOption">NoMatchOption</param>
        /// <param name="multiMatchOption">MultiMatchOption</param>
        /// <param name="useLocale">whether to use locale while getting the matched entry list </param>
        //public void SetEntryByQuery(T entry, string qualification, IModelBinder<T> binder, ARGetFormNameDelegate getFormName,
        //    bool isEnableTransaction = false, Server.SetEntryByQuery_NoMatchOption noMatchOption = Server.SetEntryByQuery_NoMatchOption.NoAction,
        //    Server.SetEntryByQuery_MultiMatchOption multiMatchOption = Server.SetEntryByQuery_MultiMatchOption.ModifyAll, bool useLocale = false)
        //{
        //    if (entry == null)
        //        throw new ArgumentNullException("entry");
        //    if (String.IsNullOrEmpty(qualification))
        //        throw new ArgumentNullException("qualification");
        //    if (binder == null)
        //        throw new ArgumentNullException("binder");
        //    if (getFormName == null)
        //        throw new ArgumentNullException("getFormName");
        //    if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
        //        throw new UnLoginException();
        //    lock (lockObject)
        //    {
        //        try
        //        {

        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.BeginBulkEntryTransaction();
        //            }

        //            loginContext.ServerInstance.SetEntryByQuery(getFormName(typeof(T)), qualification,
        //                noMatchOption, multiMatchOption, binder.UnBindForUpdate(entry), useLocale);

        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.SendBulkEntryTransaction();
        //            }
        //        }
        //        catch
        //        {
        //            if (isEnableTransaction)
        //            {
        //                loginContext.ServerInstance.CancelBulkEntryTransaction();
        //            }
        //            throw;
        //        }
        //    }

        //}

        /// <summary>
        /// Set entris by query
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entry"> entry</param>
        /// <param name="qualification"> for example: "\'cChr_ProcessType\' LIKE \"%geGen%\" AND \'cInt_ApproveOrder\'=1004"  </param>
        /// <param name="isEnableTransaction">isEnableTransaction</param>
        /// <param name="noMatchOption">NoMatchOption</param>
        /// <param name="multiMatchOption">MultiMatchOption</param>
        /// <param name="useLocale">whether to use locale while getting the matched entry list </param>
        //public void SetEntryByQuery(T entry, string qualification,
        //    bool isEnableTransaction = false, Server.SetEntryByQuery_NoMatchOption noMatchOption = Server.SetEntryByQuery_NoMatchOption.NoAction,
        //    Server.SetEntryByQuery_MultiMatchOption multiMatchOption = Server.SetEntryByQuery_MultiMatchOption.ModifyAll, bool useLocale = false)
        //{
        //    SetEntryByQuery(entry, qualification, DefaultFactory.CreateModelBinder<T>(), GetFormName, isEnableTransaction, noMatchOption, multiMatchOption, useLocale);
        //}

        
//        public object GetEntry(string entryID, Type targetModelType, IModelBinder<T> binder, ARGetFormNameDelegate getFormName,)



        /// <summary>
        /// Get AR Entry by entryId using default modelbinder and return all field in model
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entryID">entryID</param>
        public T GetEntry(string entryID)
        {
            return GetEntry(entryID, DefaultFactory.CreateModelBinder<T>());
        }


        /// <summary>
        ///  Get AR Entry by entryId using custom model binder and return all field in model
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entryID">entryID</param>
        /// <param name="binder">ModelBinder</param>
        /// <param name="getFormName">getFormName Delegate</param>
        public T GetEntry(string entryID, IModelBinder<T> binder)
        {
            return GetEntry(entryID, DefaultFactory.CreateModelBinder<T>(), binder.UnBindToFieldIdList());
        }

        /// <summary>
        /// Get AR Entry by entryId using custom model binder and custom filedIds
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entryID">entryID</param>
        /// <param name="binder">ModelBinder</param>
        /// <param name="getFormName">getFormName Delegate</param>
        public T GetEntry(string entryID, IModelBinder<T> binder,List<UInt32> FieldIds)
        {
            if (String.IsNullOrEmpty(entryID))
                throw new ArgumentNullException("entry");
            if (binder == null)
                throw new ArgumentNullException("binder");
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();
            string formName = GetFormName(typeof(T));
            lock (lockObject)
            {
                return binder.Bind(loginContext.ServerInstance.GetEntry(formName, entryID, FieldIds));
            }
        }

        /// <summary>
        /// Get Entry list by where \ select \ order by \ pagestart \ pageindex
        /// </summary>
        /// <param name="qualification">qulification , null for all . for example: "\'cChr_ProcessType\' LIKE \"%geGen%\" AND \'cInt_ApproveOrder\'=1004" </param>
        /// <param name="fieldIds">select fieldId list,null for only request id</param>
        /// <param name="StartIndex">pagestart, null for no page</param>
        /// <param name="RetrieveCount">pagecount, null for no page</param>
        /// <param name="totalMatch">ref : total count match the qulification . -1 will not cause the count, it may more efficient</param>
        /// <param name="sortInfo">sortinfo</param>
        /// <returns></returns>
        public IList<T> GetEntryList(
            string qualification,
            List<UInt32> fieldIds,
            uint? StartIndex,
            uint? RetrieveCount,
            ref int totalMatch,
            List<ARSortInfo> sortInfo
            )
        {
            return GetEntryList(qualification, fieldIds, StartIndex, RetrieveCount, ref totalMatch, sortInfo, DefaultFactory.CreateModelBinder<T>());
        }


        /// <summary>
        /// Get Entry list by where \ select \ order by \ pagestart \ pageindex
        /// </summary>
        /// <param name="qualification">qulification , null for all . for example: "\'cChr_ProcessType\' LIKE \"%geGen%\" AND \'cInt_ApproveOrder\'=1004" </param>
        /// <param name="fieldIds">select fieldId list,null for only request id</param>
        /// <param name="StartIndex">pagestart, null for no page</param>
        /// <param name="RetrieveCount">pagecount, null for no page</param>
        /// <param name="totalMatch">ref : total count match the qulification . -1 will not cause the count, it may more efficient</param>
        /// <param name="sortInfo">sortinfo</param>
        /// <param name="binder">custom modelbinder</param>
        /// <returns></returns>
        public IList<T> GetEntryList(
            string qualification,
            List<UInt32> fieldIds,
            uint? StartIndex,
            uint? RetrieveCount,
            ref int totalMatch,
            List<ARSortInfo> sortInfo,
            IModelBinder<T> binder)
        {
            if (binder == null)
                throw new ArgumentNullException("binder");
            if (loginContext.LoginStatus != ARLoginStatus.Success || loginContext.ServerInstance == null)
                throw new UnLoginException();

            string formName = GetFormName(typeof(T));

            List<AREntry> entryList = loginContext.ServerInstance.GetEntryList(
                formName,
                qualification,
                fieldIds,
                StartIndex,
                RetrieveCount,
                ref totalMatch,
                sortInfo
                );
            List<T> listT = new List<T>();
            foreach (var entry in entryList){
                string entryid = string.Join("|",entry.EntryIds.ToArray());
                T model = binder.Bind(entry.FieldValues);
                SetEntryId(model, entryid);
                listT.Add(model);
                
            }
            return listT;
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
            return GetListEntryStatictisc(Qulification, ARStat, TargetFieldId, GroupbyFieldIdList, DefaultFactory.CreateModelBinder<T>());
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
        /// <param name="binder"></param>
        /// <returns></returns>
        public IList<T> GetListEntryStatictisc(
            String Qulification,
            ARStatictisc ARStat,
            Nullable<UInt32> TargetFieldId,
            List<UInt32> GroupbyFieldIdList,
            IModelBinder<T> binder
            )
        {
            //tagetfiledid must not null if ARStat is not count
            if (TargetFieldId == null && ARStat != ARStatictisc.STAT_OP_COUNT)
                throw new InvalidOperationException("TargetFieldId must not null if ARStat is not COUNT");
            string formName = GetFormName(typeof(T));
            List<ARGroupByStatictisc> result = loginContext.ServerInstance.GetEntryListStatictisc(
                formName,
                Qulification,
                ARStat,
                TargetFieldId,
                GroupbyFieldIdList);

            List<T> models = new List<T>();
         
            //if GroupbyFieldIdList is null, the result must only one row
            //tagetfiledid can be ignore
            if (GroupbyFieldIdList == null || GroupbyFieldIdList.Count == 0)
            {
                T model = Activator.CreateInstance<T>();
                if (!(model is ARBaseForm))
                    throw new InvalidCastException("T must be inherit from ARBaseForm so that Statictisc operation can be cast.");
                (model as ARBaseForm).Statictisc = result[0].Statictisc;
                models.Add(model);
            }
            //else GroupbyFieldIdList is not null,the result may have rows more than 1
            //and targetfieldid can be ignore
            else
            {
                //store the arid and artype map
                Dictionary<uint, ARType> idmaptype = new Dictionary<uint, ARType>();

                foreach (ARGroupByStatictisc g in result){
                    T model = Activator.CreateInstance<T>();
                    List<ARFieldValue> fv = new List<ARFieldValue>();
                    for(int i = 0 ; i < g.GroupByValues.Count ; i++)
                    {
                        if (!idmaptype.ContainsKey(GroupbyFieldIdList[i]))
                            idmaptype.Add(GroupbyFieldIdList[i], GetARTypeFromFieldId(GroupbyFieldIdList[i]));
                        fv.Add(new ARFieldValue(GroupbyFieldIdList[i], g.GroupByValues[i], (ARDataType)idmaptype[GroupbyFieldIdList[i]]));
                    }
                    //bind model property from groupvalues
                    model = binder.Bind(fv);
                    //add statictisc result to the ARBaseForm
                    if (!(model is ARBaseForm))
                        throw new InvalidCastException("T must be inherit from ARBaseForm so that Statictisc operation can be cast.");
                    (model as ARBaseForm).Statictisc = g.Statictisc;
                    models.Add(model);
                }
            }


            return models;
        }

        /// <summary>
        /// get the formname 
        /// </summary>
        /// <!--William Wang-->
        /// <returns> form name</returns>
        protected string GetFormName(Type t)
        {
            string name = string.Empty;
            var attributes = t.GetCustomAttributes(false);
            ARFormAttribute formAttribute = null;
            foreach(var attr in attributes){
                if (attr is ARFormAttribute){
                    formAttribute = (ARFormAttribute)attr;
                    break;
                }
            }

            if (formAttribute != null)
                name = formAttribute.FormName;

            return name;
        }

        /// <summary>
        /// get the entry ID
        /// </summary>
        /// <!--William Wang-->
        /// <param name="entity"> model object</param>
        /// <returns></returns>
        protected string GetEntryId(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            foreach (PropertyInfo item in typeof(T).GetProperties()){
                var attributes = item.GetCustomAttributes(false);
                foreach (var attr in attributes){
                    if (attr is AREntryKeyAttribute){
                        return item.GetValue(entity, null).ToString();
                    }
                }
            }
            return string.Empty;
        }

        protected ARType GetARTypeFromFieldId(UInt32 fieldId)
        {
            foreach (PropertyInfo item in typeof(T).GetProperties()){
                var attributes = item.GetCustomAttributes(false);
                foreach (var attr in attributes)
                {
                    if (attr is ARFieldAttribute)
                    {
                        if (((ARFieldAttribute)attr).DatabaseID == fieldId)
                            return ((ARFieldAttribute)attr).DataType; 
                    }
                }
            }
            throw new InvalidOperationException(string.Format("fieldId : {0} can not be found in Model definition : {1}",fieldId,typeof(T).FullName));
        }

        /// <summary>
        /// Set Entry id for Model
        /// </summary>
        /// <param name="model">model not null</param>
        /// <param name="entryId">entryid to be set</param>
        protected void SetEntryId(T model,string entryId)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (string.IsNullOrEmpty(entryId))
                throw new ArgumentNullException("entryId");
            foreach (PropertyInfo item in typeof(T).GetProperties())
            {
                var attributes = item.GetCustomAttributes(false);
                foreach (var attr in attributes)
                {
                    if (attr is AREntryKeyAttribute)
                    {
                        item.SetValue(model, entryId, null);
                        return;
                    }
                }
            }
        }

    }

}
