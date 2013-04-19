#include "../h/ARBulkResult.h"

namespace ARNative {

ARBulkResult::ARBulkResult(void)
{

}

ARBulkResult^ ARBulkResult::ConstructARBulkResult(const ARBulkEntryReturn* entry)
{
	if(entry == NULL)
		return nullptr;
	ARBulkResult^ result = gcnew ARBulkResult();
	switch (entry->entryCallType)
	{
		case AR_BULK_ENTRY_CREATE:  
			result->CallType = EntryCallType::BULK_ENTRY_CREATE;
			result->Status = ARException::ConstructARException(&(entry->u.createEntryReturn.status));
			if(result->Status == nullptr)
				result->EntryId = MarshalCharCopyToString(entry->u.createEntryReturn.entryId);
			break;
		case AR_BULK_ENTRY_SET:
			result->CallType = EntryCallType::BULK_ENTRY_SET;
			result->Status = ARException::ConstructARException(&(entry->u.setEntryReturn));
			break;
		case AR_BULK_ENTRY_DELETE:
			result->CallType = EntryCallType::BULK_ENTRY_DELETE;
			result->Status = ARException::ConstructARException(&(entry->u.deleteEntryReturn));
			break;
		//case AR_BULK_ENTRY_MERGE
		//case AR_BULK_ENTRY_XMLCREATE
		//case AR_BULK_ENTRY_XMLSET   
		//case AR_BULK_ENTRY_XMLDELETE
		default:
			throw gcnew Exception("entryCallType is out of support.");
	}

	return result;
}

}
