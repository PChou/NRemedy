#include "../h/_Free.h"

void _FreeARStatusList(ARStatusList *, ARBoolean)
{
	return;
}

void _FreeARStatisticsResultList(ARStatisticsResultList * , ARBoolean)
{
	return;
}

void _FreeARQualifierStruct(ARQualifierStruct* , ARBoolean)
{
	return;
}

void _FreeARFieldValueOrArithStruct(ARFieldValueOrArithStruct* s, ARBoolean)
{
#ifdef _DEBUG
	if(s != NULL)
		assert(s->tag != AR_VALUE && s->tag != AR_ARITHMETIC && s->tag != AR_STAT_HISTORY && s->tag != AR_VALUE_SET && s->tag != AR_CURRENCY_FLD);
#endif
	return;
}

void _FreeAREntryListFieldValueList(AREntryListFieldValueList* , ARBoolean)
{
	return;
}

void _FreeARLocStruct(ARLocStruct* , ARBoolean)
{
	//TODO?
	return;
}

//TODO : review and add other datatype
void _FreeARFieldValueList(ARFieldValueList* fieldValueList , ARBoolean freeSelf)
{
	if(fieldValueList == NULL)
		return;
	for(int i = 0;i < fieldValueList->numItems ; i++)
	{
		switch (fieldValueList->fieldValueList[i].value.dataType)
		{
		case AR_DATA_TYPE_NULL           : /* code for a NULL value */
		case AR_DATA_TYPE_KEYWORD        : /* code indicating a keyword setting */
		case AR_DATA_TYPE_INTEGER        : /* codes for the data type of a value */
		case AR_DATA_TYPE_REAL           :
		case AR_DATA_TYPE_ENUM           :
		case AR_DATA_TYPE_TIME           :
		case AR_DATA_TYPE_BITMASK        :
		case AR_DATA_TYPE_DATE          :
		case AR_DATA_TYPE_TIME_OF_DAY   :
			break;
		//TODO these need impelement
		case AR_DATA_TYPE_BYTES          :
			break;
		case AR_DATA_TYPE_ATTACH        :
			{
				ARAttachStruct* _struct = fieldValueList->fieldValueList[i].value.u.attachVal;
				if(_struct->name != NULL)
				{
					delete[] _struct->name;
					_struct->name = NULL;
				}
				if(_struct->loc.locType == AR_LOC_FILENAME)
				{
					if(_struct->loc.u.filename != NULL)
					{
						delete[] _struct->loc.u.filename;
						_struct->loc.u.filename = NULL;
					}
				}
				else if(_struct->loc.locType == AR_LOC_BUFFER)
				{
					if(_struct->loc.u.buf.buffer != NULL)
					{
						delete[] _struct->loc.u.buf.buffer;
						_struct->loc.u.buf.buffer = NULL;
					}
				}

				delete _struct;

				break;
			}
		case AR_DATA_TYPE_CURRENCY      :
			break;
		case AR_DATA_TYPE_CHAR           :
			if(fieldValueList->fieldValueList[i].value.u.charVal != NULL){
				delete[] fieldValueList->fieldValueList[i].value.u.charVal;
				fieldValueList->fieldValueList[i].value.u.charVal = NULL;
			}
			break;
		case AR_DATA_TYPE_DIARY          :
			if(fieldValueList->fieldValueList[i].value.u.diaryVal != NULL){
				delete[] fieldValueList->fieldValueList[i].value.u.diaryVal;
				fieldValueList->fieldValueList[i].value.u.diaryVal = NULL;
			}
			break;
		case AR_DATA_TYPE_DECIMAL       :
			if(fieldValueList->fieldValueList[i].value.u.decimalVal != NULL){
				delete[] fieldValueList->fieldValueList[i].value.u.decimalVal;
				fieldValueList->fieldValueList[i].value.u.decimalVal = NULL;
			}
			break;
		default:
			break;
		}
	}

	delete[] fieldValueList->fieldValueList;
	fieldValueList->fieldValueList = NULL;

	if(freeSelf == TRUE){
		delete fieldValueList;
		fieldValueList = NULL;
	}
}

void _FreeAREntryIdList(AREntryIdList* entryIdList, ARBoolean freeSelf)
{
	if(entryIdList == NULL)
		return;
	/*AREntryIdType* ptmp = entryIdList->entryIdList;
	for( int i = 0 ; i < entryIdList->numItems ; i++ )
	{
		if(*ptmp != NULL)
			delete[] *ptmp;
		ptmp++;
	}
	delete[] ptmp;*/

	delete[] entryIdList->entryIdList;
	entryIdList->entryIdList = NULL;

	if(freeSelf == TRUE){
		delete entryIdList;
		entryIdList = NULL;
	}
}

void _FreeARInternalIdList(ARInternalIdList* internalIdList, ARBoolean freeSelf)
{
	if(internalIdList == NULL)
		return;
	delete[] internalIdList->internalIdList;
	internalIdList->internalIdList = NULL;

	if(freeSelf == TRUE){
		delete internalIdList;
		internalIdList = NULL;
	}
}

void _FreeAREntryListFieldList(AREntryListFieldList* entryListFieldList, ARBoolean freeSelf)
{
	if(entryListFieldList == NULL)
		return;
	delete[] entryListFieldList->fieldsList;
	entryListFieldList->fieldsList = NULL;

	if(freeSelf == TRUE){
		delete entryListFieldList;
		entryListFieldList = NULL;
	}
}

void _FreeARSortList(ARSortList* sortList, ARBoolean freeSelf)
{
	if(sortList == NULL)
		return;
	delete[] sortList->sortList;
	sortList->sortList = NULL;

	if(freeSelf == TRUE){
		delete sortList;
		sortList = NULL;
	}
}

void _FreeARServerInfoRequestList(ARServerInfoRequestList* infoList, ARBoolean freeSelf)
{
	if(infoList == NULL)
		return;
	delete[] infoList->requestList;
	infoList->requestList = NULL;
	infoList->numItems = 0;

	if(freeSelf == TRUE){
		delete infoList;
		infoList = NULL;
	}
}