#ifndef _AR_TRANSACTION_RESULT_H_
#define _AR_TRANSACTION_RESULT_H_

#include "ARBulkResult.h"

using namespace System;
using namespace System::Collections::Generic;

namespace ARNative{

#pragma once
public ref class ARTransactionResult
{
public:
	ARTransactionResult(void){}
public:
	property Boolean Success;
	property List<ARBulkResult^>^ ResultList;
};


}

#endif