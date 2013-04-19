//this is util functions for any other module
#ifndef _UTIL_H_
#define _UTIL_H_

#include <stdlib.h>
#include <string.h>
#include "../../api/include/ar.h"
#include "ARAttachment.h"
//for marshal between managed to native
//#include <msclr\marshal_cppstd.h>
//#include <msclr\marshal.h>
#include <yvals.h> // warning numbers get enabled in yvals.h

#pragma warning(disable:4018)

//Uitl #define

#define AR_OPERATION_FIALED(fn,args)			fn##args >= AR_RETURN_ERROR

//input : List<ARFieldValue^>^
//output : IntelligentARStructLocal<ARFieldValueList>
#define INIT_AR_FIELD_VALUE_LIST(input,output) \
															(&output)->numItems = input->Length;	\
															(&output)->fieldValueList = new ARFieldValueStruct[input->Length];	\
															ARFieldValueStruct* pStruct = (&output)->fieldValueList;	\
															for each(ARFieldValue^ ARFVL in input){ \
																ARFieldValue::ConstructARFieldValueStruct(pStruct,ARFVL);\
																pStruct++;	\
															}	\
//input : List<String^>^
//output : IntelligentARStructLocal<AREntryIdList>
#define INIT_AR_ENTRY_ID_LIST(input,output) \
															(&output)->numItems = input->Length;	\
															(&output)->entryIdList = new AREntryIdType[input->Length];	\
															AREntryIdType* pEntryId = (&output)->entryIdList;	\
															for each(String^ entryId in input){	\
																MarshalStringCopyToCharStack(*pEntryId,entryId);	\
																pEntryId++;	\
															}	\

//input : List<UInt32^>^
//output : IntelligentARStructLocal<ARInternalIdList>
#define INIT_AR_INTERNAL_ID_LIST(input,output) \
															(&output)->numItems = input->Length;	\
															(&output)->internalIdList = new ARInternalId[input->Length];	\
															ARInternalId* pFId = (&output)->internalIdList;	\
															for each(Int32 Id in input){ \
																*pFId = Id;	\
																pFId++;	\
															}	\



using namespace System;
//using namespace msclr::interop;

namespace ARNative {

	public enum class ARDataType { 
		DATA_TYPE_NULL=           0, //Object will be ignore
		DATA_TYPE_KEYWORD=        1, //Object should be UInt32
		DATA_TYPE_INTEGER=        2, //Object should be Int32
		DATA_TYPE_REAL=           3, //Object should be Double
		DATA_TYPE_CHAR=           4, //Object should be String
		DATA_TYPE_DIARY=          5, //Object should be String , repersent a encoded diary
									 //additional impelement can be used to convert
		DATA_TYPE_ENUM=           6, //Object should be Int32
		DATA_TYPE_TIME=           7, //Object should be Int32 which is the second from 1970-1-1,base on UTC time
		DATA_TYPE_BITMASK=        8, //Object should be UInt32
		DATA_TYPE_BYTES=          9, //Not impelement yet
		DATA_TYPE_DECIMAL=       10, //Object should be String , repersent a decimal string like "123.456"
		DATA_TYPE_ATTACH=        11, //Not impelement yet
		DATA_TYPE_CURRENCY=      12, //Not impelement yet
		DATA_TYPE_DATE=          13, //Object should be Int32 which is the integer number of days since Julian Day, 
									 //additional impelement can be used to convert
		DATA_TYPE_TIME_OF_DAY=   14  //Object should be Int32 which is the integer number of seconds since 12:00:00 a.m
	};

	public enum class ARStatictisc { 
		STAT_OP_COUNT=          1,
		STAT_OP_SUM=			2,
		STAT_OP_AVERAGE=		3,
		STAT_OP_MINIMUM=        4,
		STAT_OP_MAXIMUM=        5
	};

	public enum class SortOrder{
		SORT_ASCENDING=        1,  /* tags for sort order */
		SORT_DESCENDING=       2
	};

	public enum class EntryCallType{
		BULK_ENTRY_CREATE=				1,
		BULK_ENTRY_SET=                 2,
		BULK_ENTRY_DELETE=				3,
		/*BULK_ENTRY_MERGE=               4,
		BULK_ENTRY_XMLCREATE=           5,
		BULK_ENTRY_XMLSET=				6,
		BULK_ENTRY_XMLDELETE=			7*/
	};

//copy String to char safely, but invoker should response to free char*
extern char* MarshalStringCopyToChar(String^ Source);
//copy String to char unsafely,invoker should garantee the char* is safe to contain the string,usually for stack char[]
extern void MarshalStringCopyToCharStack(char* Des,String^ Source);
//convert to String^ from char* with '\0' termination
extern String^ MarshalCharCopyToString(const char* Source);
extern Object^ ARValueStructToObj(const ARValueStruct& arValueStruct);
extern void ObjToARValueStruct(ARValueStruct &des,Object^ src,ARDataType dataType);

}
#endif