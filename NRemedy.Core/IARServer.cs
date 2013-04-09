//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-04-10
//------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ARNative;

namespace NRemedy
{
    public interface IARServer
    {
        void Login(string server, string user, string password, string authentication);
        void Logout();
        //ArrayList GetListForm();
        //ARForm GetForm(string name);
        //ArrayList GetListField(string formName);
        //Field GetField(string formName, uint id);

        //void BeginBulkEntryTransaction();
        //ArrayList CancelBulkEntryTransaction();
        //ArrayList SendBulkEntryTransaction();

        string CreateEntry(string form, List<ARFieldValue> fieldValueList);
        void DeleteEntry(string form, string entryId, int deleteOption = 0);
        void SetEntry(string form, string entryId, List<ARFieldValue> fieldValueList);
        List<ARFieldValue> GetEntry(string form, string entryId, List<uint> FieldIdList);
        List<AREntry> GetEntryList(
				String SchemaName,//form name
				String Qulification,//qulification,null for all
				List<UInt32> FieldIdList,//target fieldids,null for only request id
				Nullable<UInt32> StartIndex,//page start,null for 0
				Nullable<UInt32> RetrieveCount,//page size,null for all matched
				ref Int32 totalMatch,//total size,-1 will not cause the count, it may more efficient ,see page 281
				List<ARSortInfo> SortInfo //sort info,null for no sort
			);

        List<ARGroupByStatictisc> GetEntryListStatictisc(
            String SchemaName,//form name
            String Qulification,//qulification ,null for all
            ARStatictisc ARStat,//ARStatictisc
            Nullable<UInt32> TargetFieldId,//Statictisc target fieldid, for count can be null,other can't be null
            List<UInt32> GroupbyFieldIdList //group by fieldid, null for no group
            );

        void SetImpersonatedUser(string user);
    }
}
