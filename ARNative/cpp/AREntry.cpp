#include "../h/AREntry.h"

namespace ARNative {

AREntry::AREntry(void)
{
}

AREntry^ AREntry::ConstructAREntry(const AREntryListFieldValueStruct* entry)
{
	if(entry == NULL)
		return nullptr;
	AREntry^ rEntry = gcnew AREntry();
	//entryid
	rEntry->EntryIds = gcnew List<String^>();
	int i;
	for(i = 0 ; i < entry->entryId.numItems ; i++ )
	{
		String^ t = MarshalCharCopyToString(entry->entryId.entryIdList[i]);
		rEntry->EntryIds->Add(t);
	}

	//value
	rEntry->FieldValues = gcnew List<ARFieldValue^>();
	for(i = 0 ; i < entry->entryValues->numItems ; i++ )
	{
		ARFieldValue^ fieldValue = ARFieldValue::ConstructARFieldValue(&(entry->entryValues->fieldValueList[i]));
		rEntry->FieldValues->Add(fieldValue);
	}

	return rEntry;
}

}
