#include "../h/ARSession.h"

namespace ARNative{


ARSession::ARSession(void)
{
	//init the session as NULL
	session = NULL;
	//init ansiEncodeName
	//if(ARSession::ansiEncodeName == NULL){
	//	ARSession::ansiEncodeName = new char[AR_MAX_LANG_SIZE + 1];
	//	String^ AnsiEncodeName = System::Text::Encoding::Default->BodyName;
	//	MarshalStringCopyToChar(ARSession::ansiEncodeName,AnsiEncodeName);
	//}
}


ARSession::~ARSession(void)
{
	//Free the session , if not NULL
	LogOut();

}

Boolean ARSession::HasSession()
{
	if(session == NULL)
		return false;
	return true;
}

void ARSession::Login(String^ Server,String^ User,String^ Password)
{
	//first time login
	if(session == NULL)
	{
		IntelligentARStructAR<ARStatusList> status;
		session = new ARControlStruct();
		//set client encoding see C-Reference API on page 396
		//actually the default encoding is also ANSI
		memset(session,'\0',sizeof(ARControlStruct));
		
		//strcpy(session->localeInfo.charSet,ARSession::ansiEncodeName);
		strcpy(session->localeInfo.charSet,"UTF-8");
		//set server , user , pwd
		MarshalStringCopyToCharStack(session->server,Server);
		MarshalStringCopyToCharStack(session->user,User);
		MarshalStringCopyToCharStack(session->password,Password);

		//Connect to AR
		if (AR_OPERATION_FIALED(ARInitialization,(session,&status))){
			delete this->session;
			this->session = NULL;
			throw ARException::ConstructARException(&status);
		}

		if (AR_OPERATION_FIALED(ARVerifyUser,(session, NULL,NULL,NULL,&status))) {
			delete this->session;
			this->session = NULL;
			throw ARException::ConstructARException(&status);
		}
	}
}

void ARSession::LogOut()
{
	//session has been opened before
	if(session != NULL)
	{
		ARStatusList status;
		ARTermination(this->session,&status);
		FreeARStatusList(&status,FALSE);
		delete session;
		session = NULL;
	}
}

String^ ARSession::CreateEntry(String^ SchemaName,array<ARFieldValue^>^ FieldValueList)
{
	if(SchemaName == nullptr || SchemaName == String::Empty)
		throw gcnew Exception("SchemaName should not be null.");
	if(FieldValueList == nullptr || FieldValueList->Length == 0)
		return nullptr;
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");

	IntelligentARStructAR<ARStatusList> statuslist;				//auto ~

	char n_schemaName[AR_MAX_NAME_SIZE + 1];
	//set schemaName
	MarshalStringCopyToCharStack(n_schemaName,SchemaName);

	IntelligentARStructLocal<ARFieldValueList> n_FieldValueList;//auto ~
	INIT_AR_FIELD_VALUE_LIST(FieldValueList,n_FieldValueList)

	AREntryIdType n_EntryId;

	if(AR_OPERATION_FIALED(ARCreateEntry,(
		this->session,			//session
		n_schemaName,			//schemaName	
		&n_FieldValueList,		//FieldValueList
		n_EntryId,				//entryId will be returned
		&statuslist				//statuslist will be returned
		)))
	{
		throw ARException::ConstructARException(&statuslist);
	}

	return MarshalCharCopyToString(n_EntryId);
}

void ARSession::DeleteEntry(String^ SchemaName,array<String^>^ EntryIdList)
{
	if(SchemaName == nullptr || SchemaName == String::Empty)
		throw gcnew Exception("SchemaName should not be null.");
	if(EntryIdList == nullptr || EntryIdList->Length == 0)
		return;
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");

	IntelligentARStructAR<ARStatusList> statuslist; //auto ~
	char n_schemaName[AR_MAX_NAME_SIZE + 1];
	MarshalStringCopyToCharStack(n_schemaName,SchemaName);

	IntelligentARStructLocal<AREntryIdList> IdList;
	INIT_AR_ENTRY_ID_LIST(EntryIdList,IdList)

	if(AR_OPERATION_FIALED(ARDeleteEntry,(
		this->session,				//session
		n_schemaName,				//form name
		&IdList,					//in entryid list
		AR_JOIN_DELOPTION_NONE,		//delete option
		&statuslist					//out statius list
		)))
	{
		//for the message number 302 means entryid dose not exist, here should not throw exception
		//and return null;
		if((&statuslist)->numItems > 0 && (&statuslist)->statusList[0].messageNum == 302)
			return;
		throw ARException::ConstructARException(&statuslist);
	}
}

List<ARFieldValue^>^ ARSession::GetEntry(String^ SchemaName,array<String^>^ EntryIdList, array<UInt32>^ FieldIdList)
{
	if(SchemaName == nullptr || SchemaName == String::Empty)
		throw gcnew Exception("SchemaName should not be null.");
	if(EntryIdList == nullptr || EntryIdList->Length == 0)
		return nullptr;
	if(FieldIdList == nullptr || FieldIdList->Length == 0)
		return nullptr;
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");

	IntelligentARStructAR<ARStatusList> statuslist;//auto ~

	//ARNameType
	char n_schemaName[AR_MAX_NAME_SIZE + 1];
	MarshalStringCopyToCharStack(n_schemaName,SchemaName);

	IntelligentARStructLocal<AREntryIdList> IdList;//auto ~
	INIT_AR_ENTRY_ID_LIST(EntryIdList,IdList)

	IntelligentARStructLocal<ARInternalIdList> n_FieldIdList;//auto ~
	INIT_AR_INTERNAL_ID_LIST(FieldIdList,n_FieldIdList)

	IntelligentARStructAR<ARFieldValueList> retList;//auto ~

	if(AR_OPERATION_FIALED(ARGetEntry,(
		this->session,	//session
		n_schemaName,	//form name
		&IdList,		//target request id list,only one if regular form,multiple for join form
		&n_FieldIdList,	//request field id list
		&retList,		//result fieldValueList return by api dll
		&statuslist)))
	{
		//for the message number 302 means entryid dose not exist, here should not throw exception
		//and return null;
		if((&statuslist)->numItems > 0 && (&statuslist)->statusList[0].messageNum == 302)
			return nullptr;
		throw ARException::ConstructARException(&statuslist);
	}

	List<ARFieldValue^>^ retFieldValue = gcnew List<ARFieldValue^>();
	ARFieldValueStruct* tmp = (&retList)->fieldValueList;
	for( int i = 0 ; i < (&retList)->numItems ; i++, tmp++){

		//attchement type need invoke getentryblob to get buffer see C API page 252
		if(tmp->value.dataType == AR_DATA_TYPE_ATTACH){

			IntelligentARStructAR<ARStatusList> _statuslist;//auto ~

			tmp->value.u.attachVal->loc.locType = AR_LOC_BUFFER;

			if(AR_OPERATION_FIALED(ARGetEntryBLOB,(
				this->session,	//session
				n_schemaName,	//form name
				&IdList,		//target request id list,only one if regular form,multiple for join form
				tmp->fieldId,	//request field id list
				&(tmp->value.u.attachVal->loc),		//result
				&_statuslist)))
			{
				throw ARException::ConstructARException(&_statuslist);
			}
		}//after the invoke tmp->value.u.attachVal->loc contains the buffer size and buffer data

		ARFieldValue^ fieldValue = ARFieldValue::ConstructARFieldValue(tmp);
		retFieldValue->Add(fieldValue);
	}

	return retFieldValue;
}

void ARSession::SetEntry(String^ SchemaName,array<String^>^ EntryIdList,array<ARFieldValue^>^ FieldValueList)
{
	if(SchemaName == nullptr || SchemaName == String::Empty)
		throw gcnew Exception("SchemaName should not be null.");
	if(EntryIdList == nullptr || EntryIdList->Length == 0)
		return;
	if(FieldValueList == nullptr || FieldValueList->Length == 0)
		return;
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");

	IntelligentARStructAR<ARStatusList> statuslist;//auto ~

	//ARNameType
	char n_schemaName[AR_MAX_NAME_SIZE + 1];
	MarshalStringCopyToCharStack(n_schemaName,SchemaName);

	IntelligentARStructLocal<AREntryIdList> IdList;//auto ~
	INIT_AR_ENTRY_ID_LIST(EntryIdList,IdList)
	
	IntelligentARStructLocal<ARFieldValueList> n_FieldValueList;//auto ~
	INIT_AR_FIELD_VALUE_LIST(FieldValueList,n_FieldValueList)

	if(AR_OPERATION_FIALED(ARSetEntry,(
		this->session,		//session
		n_schemaName,					//in form name
		&IdList,					//in entryidlist
		&n_FieldValueList,					//in FieldValueList 
		0,					//in lastmoddate comparison
		AR_JOIN_SETOPTION_NONE,//in allow update join criteria
		&statuslist						//out statuslist
		)))
	{
		//for the message number 302 means entryid dose not exist, here should not throw exception
		//and return null;
		if((&statuslist)->numItems > 0 && (&statuslist)->statusList[0].messageNum == 302)
			return;
		throw ARException::ConstructARException(&statuslist);
	}
}

List<ARGroupByStatictisc^>^ ARSession::GetListEntryStatictisc(
	String^ SchemaName,String^ Qulification,ARStatictisc ARStat,Nullable<UInt32> TargetFieldId ,array<UInt32>^ GroupbyFieldIdList)
{
	if(SchemaName == nullptr || SchemaName == String::Empty)
		throw gcnew Exception("SchemaName should not be null.");
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");

	IntelligentARStructAR<ARStatusList> statuslist;//auto ~
	//ARNameType
	char n_schemaName[AR_MAX_NAME_SIZE + 1];
	MarshalStringCopyToCharStack(n_schemaName,SchemaName);

	//qulifier struct
	IntelligentARStructAR<ARQualifierStruct> qulifier;
	//!!if the qulifier does not init by the ARLoadARQualifierStruct API call, memory access exception sometimes may happened when FreeARQualifierStruct is called
	//this may happen when the Qulification is empty , and mixed C Runtime Lib exist at runtime
	(&qulifier)->operation = 0;
	//(&qulifier)->u.fieldId = 0;
	if(Qulification != nullptr && Qulification != String::Empty){
		char* qString = NULL;//(char*)calloc(Qulification->Length*2+2,sizeof(char));//garantee buffer enough
		qString = MarshalStringCopyToChar(Qulification);
		IntelligentARStructAR<ARStatusList> _statuslist;//auto ~
		if(AR_OPERATION_FIALED(ARLoadARQualifierStruct,(
			this->session,				//session
			n_schemaName,				//in form name
			NULL,						//in VUI Name
			qString,					//in query string
			&qulifier,					//out qulifier struct
			&_statuslist)))
		{
			delete[] qString;
			throw ARException::ConstructARException(&statuslist);
		}

		delete[] qString;
	}

	//ARInternalIdList groupByList
	IntelligentARStructLocal<ARInternalIdList> groupByList;
	if(GroupbyFieldIdList != nullptr && GroupbyFieldIdList->Length > 0)
	{
		INIT_AR_INTERNAL_ID_LIST(GroupbyFieldIdList,groupByList)
	}

	//ARFieldValueOrArithStruct target
	//TargetFieldId
	IntelligentARStructLocal<ARFieldValueOrArithStruct> target;
	if(TargetFieldId.HasValue){
		(&target)->tag = AR_FIELD;
		(&target)->u.fieldId = TargetFieldId.Value;
	}


	//ARStatisticsResultList results
	IntelligentARStructAR<ARStatisticsResultList> results;

	if(AR_OPERATION_FIALED(ARGetEntryStatistics,(
		this->session,				//session
		n_schemaName,				//form name
		(Qulification == nullptr || Qulification == String::Empty) ? NULL : &qulifier,					
									//in qulifier struct
		TargetFieldId.HasValue ? &target : NULL,						//in ARFieldValueOrArithStruct?
		(unsigned int)ARStat,		//int statistic type
		(GroupbyFieldIdList == nullptr || GroupbyFieldIdList->Length == 0) ? NULL : &groupByList,				
									//int group by list
		&results,					//out ARStatisticsResultList
		&statuslist)))
	{
		throw ARException::ConstructARException(&statuslist);
	}

	List<ARGroupByStatictisc^>^ ret = gcnew List<ARGroupByStatictisc^>();
	for ( int i = 0 ; i < (&results)->numItems ; i++ )
	{
		ARGroupByStatictisc^ stat = ARGroupByStatictisc::ConstructARStatictisc(&((&results)->resultList[i]));
		ret->Add(stat);
	}

	return ret;
}

List<AREntry^>^ ARSession::GetEntryList(
				String^ SchemaName,//form name
				String^ Qulification,//qulification,null for all
				array<UInt32>^ FieldIdList,//target fieldids,null for only request id
				Nullable<UInt32> StartIndex,//page start,null for 0
				Nullable<UInt32> RetrieveCount,//page size,null for all matched
				Int32% totalMatch,//total size
				array<ARSortInfo^>^ SortInfo //sort info,null for no sort
	)
{
	if(SchemaName == nullptr || SchemaName == String::Empty)
		throw gcnew Exception("SchemaName should not be null.");
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");

	IntelligentARStructAR<ARStatusList> statuslist;//auto ~
	//ARNameType
	char n_schemaName[AR_MAX_NAME_SIZE + 1];
	MarshalStringCopyToCharStack(n_schemaName,SchemaName);
	//qulifier struct
	IntelligentARStructAR<ARQualifierStruct> qulifier;
	//!!if the qulifier does not init by the ARLoadARQualifierStruct API call, memory access exception sometimes may happened when FreeARQualifierStruct is called
	//this may happen when the Qulification is empty , and mixed C Runtime Lib exist at runtime
	(&qulifier)->operation = 0;
	if(Qulification != nullptr && Qulification != String::Empty){
		char* qString = NULL;//(char*)calloc(Qulification->Length*2+2,sizeof(char));//garantee buffer enough
		qString = MarshalStringCopyToChar(Qulification);
		IntelligentARStructAR<ARStatusList> _statuslist;//auto ~
		if(AR_OPERATION_FIALED(ARLoadARQualifierStruct,(
			this->session,				//session
			n_schemaName,				//in form name
			NULL,						//in VUI Name
			qString,					//in query string
			&qulifier,					//out qulifier struct
			&_statuslist)))
		{
			delete[] qString;
			throw ARException::ConstructARException(&_statuslist);
		}

		delete[] qString;
	}

	//AREntryListFieldList
	IntelligentARStructLocal<AREntryListFieldList> fieldIdList;
	//the reason why add default fieldId 1 is that 
	//the api will return short description if without fieldIdList which may retrieve more network traffic
	if(FieldIdList == nullptr || FieldIdList->Length == 0)
	{
		(&fieldIdList)->numItems = 1;
		(&fieldIdList)->fieldsList = new AREntryListFieldStruct[1];
		(&fieldIdList)->fieldsList->fieldId = 1;	//defautl to get fieldId 1 which is request id
		(&fieldIdList)->fieldsList->columnWidth = 1;//greater than 0 see page 69
		strcpy((&fieldIdList)->fieldsList->separator," ");//see page 69
	}
	else
	{
		(&fieldIdList)->numItems = FieldIdList->Length;
		AREntryListFieldStruct* tmp = (&fieldIdList)->fieldsList = new AREntryListFieldStruct[FieldIdList->Length];
		for each(UInt32 Id in FieldIdList)
		{
			tmp->fieldId = Id;	//defautl to get fieldId 1 which is request id
			tmp->columnWidth = 1;//greater than 0 see page 69
			strcpy(tmp->separator," ");//see page 69
			tmp++;
		}
	}

	//ARSortList
	IntelligentARStructLocal<ARSortList> SortList;
	if(SortInfo != nullptr && SortInfo->Length != 0)
	{
		(&SortList)->numItems = SortInfo->Length;
		ARSortStruct* tmp2 = (&SortList)->sortList = new ARSortStruct[SortInfo->Length];
		for each(ARSortInfo^ sort in SortInfo)
		{
			tmp2->fieldId = sort->FieldId;
			tmp2->sortOrder = (unsigned int)sort->Order;
			tmp2++;
		}
	}

	//total matched
	unsigned int totalCount = 0;

	//AREntryListFieldValueList
	IntelligentARStructAR<AREntryListFieldValueList> retfvl;

	if(AR_OPERATION_FIALED(ARGetListEntryWithFields,(this->session,
		n_schemaName,
		(Qulification == nullptr || Qulification == String::Empty) ? NULL : &qulifier,
		&fieldIdList,
		(SortInfo == nullptr || SortInfo->Length == 0) ? NULL : &SortList,
		(StartIndex.HasValue) ?  StartIndex.Value : 0,
		(RetrieveCount.HasValue) ?  RetrieveCount.Value : 0,
		FALSE,
		&retfvl,
		(totalMatch == -1) ? NULL : &totalCount,
		&statuslist)))
	{
		throw ARException::ConstructARException(&statuslist);
	}

	List<AREntry^>^ retEntry = gcnew List<AREntry^>();
	if(totalMatch != -1)
		totalMatch = totalCount;
	//return value list
	for( int i = 0 ; i < (&retfvl)->numItems ; i++ ){

		ARFieldValueList* tmp = (&retfvl)->entryList[i].entryValues;
		for( int j = 0 ; j < tmp->numItems ; j++ )
		{
			if(tmp->fieldValueList[j].value.dataType == AR_DATA_TYPE_ATTACH)
			{
				IntelligentARStructAR<ARStatusList> _statuslist;//auto ~

				tmp->fieldValueList[j].value.u.attachVal->loc.locType = AR_LOC_BUFFER;

				if(AR_OPERATION_FIALED(ARGetEntryBLOB,(
					this->session,	//session
					n_schemaName,	//form name
					&((&retfvl)->entryList[i].entryId),		//target request id list,only one if regular form,multiple for join form
					tmp->fieldValueList[j].fieldId,	//request field id list
					&(tmp->fieldValueList[j].value.u.attachVal->loc),		//result
					&_statuslist)))
				{
					throw ARException::ConstructARException(&_statuslist);
				}
			}
		}

		AREntry^ entry = AREntry::ConstructAREntry(&((&retfvl)->entryList[i]));
		retEntry->Add(entry);
	}

	return retEntry;
}


array<Byte>^ ARSession::GetBLOB(
			String^ SchemaName,
			array<String^>^ EntryIdList,
			UInt32 AttachFieldId)
{
	if(SchemaName == nullptr || SchemaName == String::Empty)
		throw gcnew Exception("SchemaName should not be null.");
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");

	IntelligentARStructAR<ARStatusList> statuslist;//auto ~
	//ARNameType
	char n_schemaName[AR_MAX_NAME_SIZE + 1];
	MarshalStringCopyToCharStack(n_schemaName,SchemaName);

	IntelligentARStructLocal<AREntryIdList> IdList;//auto ~
	INIT_AR_ENTRY_ID_LIST(EntryIdList,IdList)

	IntelligentARStructAR<ARLocStruct> loc;
	(&loc)->locType = AR_LOC_BUFFER;


	if(AR_OPERATION_FIALED(ARGetEntryBLOB,(
		this->session,	//session
		n_schemaName,	//form name
		&IdList,		//target request id list,only one if regular form,multiple for join form
		(ARInternalId)AttachFieldId,	//request field id list
		&loc,		//result fieldValueList return by api dll
		&statuslist)))
	{
		//for the message number 302 means entryid dose not exist, here should not throw exception
		//and return null;
		if((&statuslist)->numItems > 0 && (&statuslist)->statusList[0].messageNum == 302)
			return nullptr;
		throw ARException::ConstructARException(&statuslist);
	}

	ARULong32 nlen = (&loc)->u.buf.bufSize;
	unsigned char * buffer_point = (&loc)->u.buf.buffer;
	array<Byte>^ buffer = gcnew array<Byte>(nlen);

	for(int j=0; j < nlen; j++)
		buffer[j] = buffer_point[j];
	 
	return buffer;
}

void ARSession::SetImpersonatedUser(String^ UserName)
{
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");
	IntelligentARStructAR<ARStatusList> statuslist;//auto ~
	//ARSetImpersonatedUser call will not set statuslist if call success which will cause invalid memory access
	//when FreeARStatusList by AR API.
	//So here init the statuslist to avoid this.
	(&statuslist)->numItems = 0;
	(&statuslist)->statusList = NULL;
	if(UserName != nullptr && UserName != String::Empty)
	{
		char n_UserName[AR_MAX_ACCESS_NAME_SIZE + 1];
		MarshalStringCopyToCharStack(n_UserName,UserName);
		if(AR_OPERATION_FIALED(ARSetImpersonatedUser,(
			this->session,	//session
			n_UserName,	//user name
			&statuslist)))
		{
			throw ARException::ConstructARException(&statuslist);
		}
	}
	else
	{
		if(AR_OPERATION_FIALED(ARSetImpersonatedUser,(
			this->session,	//session
			NULL,	//user name
			&statuslist)))
		{
			throw ARException::ConstructARException(&statuslist);
		}
	}
}


void ARSession::BeginBulkEntryTransaction()
{
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");
	IntelligentARStructAR<ARStatusList> statuslist;//auto ~

	if(AR_OPERATION_FIALED(ARBeginBulkEntryTransaction,(
			this->session,	//session
			&statuslist)))
	{
		throw ARException::ConstructARException(&statuslist);
	}
}


ARTransactionResult^ ARSession::EndBulkEntryTransaction(UInt32 ActionType)
{
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");
	IntelligentARStructAR<ARStatusList> statuslist;//auto ~
	IntelligentARStructAR<ARBulkEntryReturnList> returnlist;
	(&returnlist)->numItems = 0;

	ARTransactionResult^ TransactionResult = gcnew ARTransactionResult();
	TransactionResult->Success = true;

	if(AR_OPERATION_FIALED(AREndBulkEntryTransaction,(
			this->session,	//session
			ActionType,
			&returnlist,
			&statuslist)))
	{
		if((&statuslist)->numItems > 0 && (&statuslist)->statusList[0].messageNum == 9713)
			TransactionResult->Success = false;
		else
			throw ARException::ConstructARException(&statuslist);
	}

	TransactionResult->ResultList = gcnew List<ARBulkResult^>();
	int count = (&returnlist)->numItems;
	ARBulkEntryReturn* r = (&returnlist)->entryReturnList;
	for(int i = 0 ; i < count ; i++ , r++){
		TransactionResult->ResultList->Add(ARBulkResult::ConstructARBulkResult(r));
	}

	return TransactionResult;
}

List<ARServerInfo^>^ ARSession::GetServerInfo(array<UInt32>^ TypeList)
{
	if(this->session == NULL)
		throw gcnew Exception("Login should not be performed before any other operation.");
	IntelligentARStructAR<ARStatusList> statuslist;//auto ~
	IntelligentARStructLocal<ARServerInfoRequestList> inputList;
	IntelligentARStructAR<ARServerInfoList> resultList;

	(&inputList)->requestList = (unsigned int*)calloc(TypeList->Length,sizeof(unsigned int));
	(&inputList)->numItems = 0;

	for(int j = 0 ; j < TypeList->Length ; j++)
	{
		(&inputList)->requestList[j] = TypeList[j];
		(&inputList)->numItems++;
	}


	if(AR_OPERATION_FIALED(ARGetServerInfo,(
			this->session,	//session
			&inputList,
			&resultList,
			&statuslist)))
	{
		throw ARException::ConstructARException(&statuslist);
	}

	List<ARServerInfo^>^ infoes = gcnew List<ARServerInfo^>();
	int count = (&resultList)->numItems;
	ARServerInfoStruct* p = (&resultList)->serverInfoList;
	for(int i = 0; i < count ; i++, p++)
	{
		ARServerInfo^ info = ARServerInfo::ConstructARServerInfo(p);
		infoes->Add(info);
	}

	return infoes;
}


}