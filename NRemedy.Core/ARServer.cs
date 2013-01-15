//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------
using System;
using System.Collections.Generic;
using ARNative;


namespace NRemedy
{
    /// <summary>
    /// implement of IARServer,just using BMC.ARSystem.Server
    /// </summary>
    public class ARServer : IARServer
    {
        protected ARSession SessionInstance;


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
            if (SessionInstance == null)
                throw new Exception("ARSession has not been initialization.");
            return SessionInstance.CreateEntry(form, fieldValueList.ToArray());
        }

        public void DeleteEntry(string form, string entryId, int deleteOption = 0)
        {
            if (SessionInstance == null)
                throw new Exception("ARSession has not been initialization.");
            SessionInstance.DeleteEntry(form, entryId.Split('|'));
        }

        public void SetEntry(string form, string entryId, List<ARFieldValue> fieldValueList)
        {
            if (SessionInstance == null)
                throw new Exception("ARSession has not been initialization.");
            SessionInstance.SetEntry(form, entryId.Split('|'), fieldValueList.ToArray());
        }

        public List<ARFieldValue> GetEntry(string form, string entryId, List<uint> FieldIdList)
        {
            if (SessionInstance == null)
                throw new Exception("ARSession has not been initialization.");
            return SessionInstance.GetEntry(form, entryId.Split('|'), FieldIdList.ToArray());
        }


        public List<AREntry> GetEntryList(string SchemaName, string Qulification, List<uint> FieldIdList, uint? StartIndex, uint? RetrieveCount, ref int totalMatch, List<ARSortInfo> SortInfo)
        {
            if (SessionInstance == null)
                throw new Exception("ARSession has not been initialization.");
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

        public List<ARGroupByStatictisc> GetEntryListStatictisc(
            String SchemaName,
            String Qulification,
            ARStatictisc ARStat,
            Nullable<UInt32> TargetFieldId,
            List<UInt32> GroupbyFieldIdList
            )
        {
            if (SessionInstance == null)
                throw new Exception("ARSession has not been initialization.");
            if(ARStat != ARStatictisc.STAT_OP_COUNT && TargetFieldId == null)
                throw new ArgumentNullException("TargetFieldId can not be null while ARStat is not COUNT");

            return SessionInstance.GetListEntryStatictisc(SchemaName, Qulification, ARStat, TargetFieldId,
                GroupbyFieldIdList == null ? null : GroupbyFieldIdList.ToArray()
                );
 
        }

    }

}
