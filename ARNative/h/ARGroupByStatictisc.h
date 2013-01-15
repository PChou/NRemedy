#ifndef _AR_GROUP_BY_STATICTISC_H_
#define _AR_GROUP_BY_STATICTISC_H_

#include "../../api/include/ar.h"
#include "Util.h"

using namespace System;
using namespace System::Collections::Generic;

#pragma once
namespace ARNative{

public ref class ARGroupByStatictisc
{
public:
	ARGroupByStatictisc(void);
	static ARGroupByStatictisc^ ConstructARStatictisc(const ARStatisticsResultStruct* stat);
public:
	property List<Object^>^ GroupByValues;
	property Object^ Statictisc;

};

}

#endif