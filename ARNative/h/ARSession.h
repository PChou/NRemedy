#ifndef _ARSESSION_H_
#define _ARSESSION_H_

#include "../../api/include/ar.h"
#include "../../api/include/arerrno.h"
#include "../../api/include/arextern.h"
#include "../../api/include/arstruct.h"

//some util
#include "Util.h"
#include "IntelligentARStruct.h"

//managed type
#include "ARFieldValue.h"
#include "ARGroupByStatictisc.h"
#include "ARException.h"
#include "AREntry.h"
#include "ARSortInfo.h"
#include "ARBulkResult.h"
#include "ARTransactionResult.h"

//for memset function
#include <cstring>
#include <string>

using namespace System;
using namespace System::Collections::Generic;

#pragma once

namespace ARNative
{
	public ref class ARSession
	{
	public:
		ARSession(void);
		~ARSession(void);
	public:
		
		Boolean HasSession();
		void Login(String^ Server,String^ User,String^ Password);
		void LogOut();

		String^ CreateEntry(String^ SchemaName,array<ARFieldValue^>^ FieldValueList);
		//A list of values of type AREntryIdType that identify the entries. For entries of join 
		//schemas, the list contains the entry IDs for all the member forms. For other 
		//schemas, the list contains one entry ID.
		//ignore if entry dose not exist in database
		void DeleteEntry(String^ SchemaName,array<String^>^ EntryIdList);
		//A list of values of type AREntryIdType that identify the entries. For entries of join 
		//schemas, the list contains the entry IDs for all the member forms. For other 
		//schemas, the list contains one entry ID.
		//return null if entry dose not exist in database
		//return null if FieldIdList is null
		List<ARFieldValue^>^ GetEntry(String^ SchemaName,array<String^>^ EntryIdList, array<UInt32>^ FieldIdList);
		//A list of values of type AREntryIdType that identify the entries. For entries of join 
		//schemas, the list contains the entry IDs for all the member forms. For other 
		//schemas, the list contains one entry ID.
		//return null if entry dose not exist in database
		void SetEntry(String^ SchemaName,array<String^>^ EntryIdList,array<ARFieldValue^>^ FieldValueList);
		//Get List Entry for Statictisc
		//SchemaName : formName
		//Qulification : String base qulification
		//ARStat : statictisc type
		//TargetFieldId : max,min,average,sum target, null for count . ex : max(2000001)
			//actually targetFieldId can be quite complex if use operation,but here only support user to define target field id
		//GroupbyFieldIdList: group by fieldId,null for no group
		List<ARGroupByStatictisc^>^ GetListEntryStatictisc(String^ SchemaName,String^ Qulification,ARStatictisc ARStat,Nullable<UInt32> TargetFieldId,array<UInt32>^ GroupbyFieldIdList);

		//Get List Entry by qString\field\order\page
		List<AREntry^>^ GetEntryList(
				String^ SchemaName,//form name
				String^ Qulification,//qulification,null for all
				array<UInt32>^ FieldIdList,//target fieldids,null for only request id
				Nullable<UInt32> StartIndex,//page start,null for 0
				Nullable<UInt32> RetrieveCount,//page size,null for all matched
				Int32% totalMatch,//total size,-1 will not cause the count, it may more efficient ,see page 281
				array<ARSortInfo^>^ SortInfo //sort info,null for no sort
			);

		array<Byte>^ GetBLOB(
			String^ SchemaName,
			array<String^>^ EntryIdList,
			UInt32 AttachFieldId);

		void SetImpersonatedUser(String^ UserName);
		//AR Transaction API
		void BeginBulkEntryTransaction();
		ARTransactionResult^ EndBulkEntryTransaction(UInt32 ActionType);
		//
	private:
		//store the AR session
		ARControlStruct* session;
		//store the ansi Encode Name in the current enviroment, like gb2312
		//this is for the charset in ARControlStruct , in order to marshal string
		//static char* ansiEncodeName;
	};
}


#endif