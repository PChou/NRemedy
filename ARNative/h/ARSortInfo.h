#ifndef _AR_SORT_INFO_H_
#define _AR_SORT_INFO_H_

#include "Util.h"

using namespace System;


namespace ARNative {

#pragma once

public ref class ARSortInfo
{
public:
	ARSortInfo(void);
public:
	property UInt32 FieldId;
	property SortOrder Order;

};


}

#endif