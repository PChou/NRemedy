//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------
using System;
using System.Collections.Generic;
using ARNative;
using System.Threading;


namespace NRemedy
{
    /// <summary>
    /// implement of IARServer,just using BMC.ARSystem.Server
    /// </summary>
    public class ARServer : IARServer
    {
        protected ARSession SessionInstance;

        protected static int _SERVER_INFO_MAX_ENTRIES = -1;

        public static int SERVER_INFO_MAX_ENTRIES
        {
            get
            {
                return _SERVER_INFO_MAX_ENTRIES;
            }
            set
            {
                _SERVER_INFO_MAX_ENTRIES = value;
            }

        }

        private static object _lock = new object();

        protected void CheckSessionNull()
        {
            if (SessionInstance == null)
                throw new Exception("ARSession has not been initialization.");
        }

        public void Login(string server, string user, string password, string authentication)
        {
            if (SessionInstance == null)
                SessionInstance = new ARSession();
            //has been login
            if (SessionInstance.HasSession())
                return;
            //has not been login
            SessionInstance.Login(server, user, password);
        }

        public void Logout()
        {
            if (SessionInstance == null)
                return;
            SessionInstance.LogOut();
        }

        public string CreateEntry(string form, List<ARFieldValue> fieldValueList)
        {
            CheckSessionNull();
            return SessionInstance.CreateEntry(form, fieldValueList.ToArray());
        }

        public void DeleteEntry(string form, string entryId, int deleteOption = 0)
        {
            CheckSessionNull();
            SessionInstance.DeleteEntry(form, entryId.Split('|'));
        }

        public void SetEntry(string form, string entryId, List<ARFieldValue> fieldValueList)
        {
            CheckSessionNull();
            SessionInstance.SetEntry(form, entryId.Split('|'), fieldValueList.ToArray());
        }

        public List<ARFieldValue> GetEntry(string form, string entryId, List<uint> FieldIdList)
        {
            CheckSessionNull();
            return SessionInstance.GetEntry(form, entryId.Split('|'), FieldIdList.ToArray());
        }


        public List<AREntry> GetEntryList(string SchemaName, string Qulification, List<uint> FieldIdList, uint StartIndex, uint? RetrieveCount, ref int totalMatch, List<ARSortInfo> SortInfo)
        {
            CheckSessionNull();

            //try to get the SERVER_INFO_MAX_ENTRIES when first GetEntryList call
            //consider multi-thread problem
            if (SERVER_INFO_MAX_ENTRIES == -1)
            {
                //thread may suspend just here,so we had better double check the SERVER_INFO_MAX_ENTRIES
                 lock (_lock){
                    if (SERVER_INFO_MAX_ENTRIES == -1)
                    {
                        List<uint> request = new List<uint>(){ (uint)ServerInfoType.SERVER_INFO_MAX_ENTRIES };
                        var result = SessionInstance.GetServerInfo(request.ToArray());
                        SERVER_INFO_MAX_ENTRIES = (int)result[0].Value;
                    }
                }
            }
            
            //if SERVER_INFO_MAX_ENTRIES is 0 or -1, no need to cut page
            if ((SERVER_INFO_MAX_ENTRIES == 0 || SERVER_INFO_MAX_ENTRIES == -1)
                //if maxEntriesPerQuery can cover per Query,only one query needed.
                || RetrieveCount <= SERVER_INFO_MAX_ENTRIES)
            {
                return SessionInstance.GetEntryList(
                    SchemaName,
                    Qulification,
                    FieldIdList == null ? null : FieldIdList.ToArray(),
                    StartIndex,
                    RetrieveCount,
                    ref totalMatch,
                    SortInfo == null ? null : SortInfo.ToArray()
                    );
            }
            else
            {
                List<AREntry> list = new List<AREntry>();
                //if StartIndex or RetrieveCount is null, all records should be return
                if (StartIndex == null || RetrieveCount == null)
                {
                    uint startIndex = StartIndex;
                    int totalm = -1;
                    do
                    {
                        var l = SessionInstance.GetEntryList(SchemaName, Qulification,
                            FieldIdList == null ? null : FieldIdList.ToArray(),
                            startIndex, (uint)SERVER_INFO_MAX_ENTRIES, ref totalm, //loop the page
                            SortInfo == null ? null : SortInfo.ToArray());
                        if (l.Count <= 0)
                            break;
                        list.AddRange(l);
                        startIndex += (uint)SERVER_INFO_MAX_ENTRIES;
                    }
                    while (true);
                    if (totalMatch != -1)
                        totalMatch = list.Count;

                }
                //if both StartIndex and RetrieveCount is not null, only one page should be return
                else
                {
                    uint startIndex2 = (uint)StartIndex;
                    //we must seperaly use the parameter : totalMatch, in order to get the totalMatch once in this case
                    var l2 = SessionInstance.GetEntryList(SchemaName, Qulification,
                        FieldIdList == null ? null : FieldIdList.ToArray(),
                        startIndex2, (uint)SERVER_INFO_MAX_ENTRIES, ref totalMatch, 
                        SortInfo == null ? null : SortInfo.ToArray());
                    int reservedCount = (int)RetrieveCount - SERVER_INFO_MAX_ENTRIES;
                    startIndex2 += (uint)SERVER_INFO_MAX_ENTRIES;
                    list.AddRange(l2);
                    int tlm = -1; //avoid count(*) multiple times
                    while (reservedCount > 0)
                    {
                        List<AREntry> l3;
                        if (reservedCount > SERVER_INFO_MAX_ENTRIES)
                        {
                            l3 = SessionInstance.GetEntryList(SchemaName, Qulification,
                                    FieldIdList == null ? null : FieldIdList.ToArray(),
                                    startIndex2, (uint)SERVER_INFO_MAX_ENTRIES, ref tlm,
                                    SortInfo == null ? null : SortInfo.ToArray());
                            reservedCount = reservedCount - SERVER_INFO_MAX_ENTRIES;
                            startIndex2 += (uint)SERVER_INFO_MAX_ENTRIES;
                        }
                        else
                        {
                            l3 = SessionInstance.GetEntryList(SchemaName, Qulification,
                                    FieldIdList == null ? null : FieldIdList.ToArray(),
                                    startIndex2, (uint)reservedCount, ref tlm,
                                    SortInfo == null ? null : SortInfo.ToArray());
                            reservedCount = 0;
                        }
                        list.AddRange(l3);
                    }
 
                }

                return list;
 
            }


        }

        public List<ARGroupByStatictisc> GetEntryListStatictisc(
            String SchemaName,
            String Qulification,
            ARStatictisc ARStat,
            Nullable<UInt32> TargetFieldId,
            List<UInt32> GroupbyFieldIdList
            )
        {
            CheckSessionNull();
            if(ARStat != ARStatictisc.STAT_OP_COUNT && TargetFieldId == null)
                throw new ArgumentNullException("TargetFieldId can not be null while ARStat is not COUNT");

            return SessionInstance.GetListEntryStatictisc(SchemaName, Qulification, ARStat, TargetFieldId,
                GroupbyFieldIdList == null ? null : GroupbyFieldIdList.ToArray()
                );
 
        }

        public void SetImpersonatedUser(string user)
        {
            SessionInstance.SetImpersonatedUser(user);
        }



        public void BeginBulkEntryTransaction()
        {
            CheckSessionNull();
            SessionInstance.BeginBulkEntryTransaction();
        }

        public ARTransactionResult CommitBulkEntryTransaction()
        {
            CheckSessionNull();
            return SessionInstance.EndBulkEntryTransaction(1);
        }

        public void CancelBulkEntryTransaction()
        {
            CheckSessionNull();
            SessionInstance.EndBulkEntryTransaction(2);
        }

        public List<ARGroupInfo> GetListGroup(string user)
        {
            CheckSessionNull();
            return SessionInstance.GetUserGroupList(user);
        }
    }

}
