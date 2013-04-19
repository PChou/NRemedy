#ifndef _AR_BULK_RESULT_H_
#define _AR_BULK_RESULT_H_

#include "Util.h"
#include "ARException.h"

using namespace System;
using namespace System::Collections::Generic;

namespace ARNative{

#pragma once
public ref class ARBulkResult
{
public:
	ARBulkResult(void);
	static ARBulkResult^ ConstructARBulkResult(const ARBulkEntryReturn* entry);
public:
	property EntryCallType CallType;
	property String^ EntryId;
	property ARException^ Status;
};


}

#endif