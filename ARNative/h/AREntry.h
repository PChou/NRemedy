#ifndef _AR_ENTRY_H_
#define _AR_ENTRY_H_

#include "ARFieldValue.h"
#include "Util.h"

using namespace System;
using namespace System::Collections::Generic;

namespace ARNative{

#pragma once
public ref class AREntry
{
public:
	AREntry(void);
	static AREntry^ ConstructAREntry(const AREntryListFieldValueStruct* entry);
public:
	property List<String^>^ EntryIds;
	property List<ARFieldValue^>^ FieldValues;
};


}

#endif