//this class is designed to free ar struct intelligent
//just for Stack struct
#ifndef _INTELLIGENT_AR_STRUCT_H_
#define _INTELLIGENT_AR_STRUCT_H_

#include "../../api/include/ar.h"
#include "../../api/include/arfree.h"
#include "_Free.h"
#include <typeinfo>

#ifdef _DEBUG
#include <assert.h>
#endif


template <typename T>
public class IntelligentARStructAR
{
public:
	IntelligentARStructAR(){}
	~IntelligentARStructAR()
	{
		const char* typeName = typeid(T).name();
		if(strcmp(typeName,"struct ARStatusList") == 0)
			FreeARStatusList((ARStatusList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARFieldValueList") == 0)
			FreeARFieldValueList((ARFieldValueList*)&_Struct,False);
		else if(strcmp(typeName,"struct AREntryIdList") == 0)
			FreeAREntryIdList((AREntryIdList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARInternalIdList") == 0)
			FreeARInternalIdList((ARInternalIdList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARStatisticsResultList") == 0)
			FreeARStatisticsResultList((ARStatisticsResultList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARQualifierStruct") == 0)
			FreeARQualifierStruct((ARQualifierStruct*)&_Struct,False);
		else if(strcmp(typeName,"struct ARFieldValueOrArithStruct") == 0)
			FreeARFieldValueOrArithStruct((ARFieldValueOrArithStruct*)&_Struct,False);
		else if(strcmp(typeName,"struct AREntryListFieldList") == 0)
			FreeAREntryListFieldList((AREntryListFieldList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARSortList") == 0)
			FreeARSortList((ARSortList*)&_Struct,False);
		else if(strcmp(typeName,"struct AREntryListFieldValueList") == 0)
			FreeAREntryListFieldValueList((AREntryListFieldValueList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARLocStruct") == 0)
			FreeARLocStruct((ARLocStruct*)&_Struct,False);
		else if(strcmp(typeName,"struct ARBulkEntryReturnList") == 0)
			FreeARBulkEntryReturnList((ARBulkEntryReturnList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARServerInfoList") == 0)
			FreeARServerInfoList((ARServerInfoList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARGroupInfoList") == 0)
			FreeARGroupInfoList((ARGroupInfoList*)&_Struct,False);
#ifdef _DEBUG
		else //always should not be here,which means no suitable FreeFunction
			assert(strcmp(typeName,"Missing Type FreeFunction.") == 0);
#endif
	}
	T* operator &(void) { return &_Struct; }
private:
	T _Struct;
};

template <typename T>
public class IntelligentARStructLocal
{
public:
	IntelligentARStructLocal(){
		//garantee there is no invalid pointer
		memset((void*)&_Struct,0,sizeof(T));
	}
	~IntelligentARStructLocal()
	{
		const char* typeName = typeid(T).name();
		if(strcmp(typeName,"struct ARStatusList") == 0)
			_FreeARStatusList((ARStatusList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARFieldValueList") == 0)
			_FreeARFieldValueList((ARFieldValueList*)&_Struct,False);
		else if(strcmp(typeName,"struct AREntryIdList") == 0)
			_FreeAREntryIdList((AREntryIdList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARInternalIdList") == 0)
			_FreeARInternalIdList((ARInternalIdList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARStatisticsResultList") == 0)
			_FreeARStatisticsResultList((ARStatisticsResultList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARQualifierStruct") == 0)
			_FreeARQualifierStruct((ARQualifierStruct*)&_Struct,False);
		else if(strcmp(typeName,"struct ARFieldValueOrArithStruct") == 0)
			_FreeARFieldValueOrArithStruct((ARFieldValueOrArithStruct*)&_Struct,False);
		else if(strcmp(typeName,"struct AREntryListFieldList") == 0)
			_FreeAREntryListFieldList((AREntryListFieldList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARSortList") == 0)
			_FreeARSortList((ARSortList*)&_Struct,False);
		else if(strcmp(typeName,"struct AREntryListFieldValueList") == 0)
			_FreeAREntryListFieldValueList((AREntryListFieldValueList*)&_Struct,False);
		else if(strcmp(typeName,"struct ARLocStruct") == 0)
			_FreeARLocStruct((ARLocStruct*)&_Struct,False);
		else if(strcmp(typeName,"struct ARServerInfoRequestList") == 0)
			_FreeARServerInfoRequestList((ARServerInfoRequestList*)&_Struct,False);
#ifdef _DEBUG
		else//always should not be here,which means no suitable _FreeFunction
			assert(strcmp(typeName,"Missing Type FreeFunction.") == 0);
#endif
	}
	T* operator &(void) { return &_Struct; }
private:
	T _Struct;
};

#endif