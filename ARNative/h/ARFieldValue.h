#ifndef _AR_FIELD_VALUE_H_
#define _AR_FIELD_VALUE_H_

#include "../../api/include/ar.h"
#include "Util.h"


using namespace System;

#pragma once
namespace ARNative {

//indicate the ARFieldValueStruct in managed Code
public ref class ARFieldValue
{
public:
	ARFieldValue(void);
	ARFieldValue(UInt32 fieldId,Object^ value,ARDataType dataType);
	static ARFieldValue^ ConstructARFieldValue(const ARFieldValueStruct* fvs);
	static void ConstructARFieldValueStruct(ARFieldValueStruct* des , ARFieldValue^ src);
public:
	property UInt32 FieldId;
	property Object^ Value;
	property ARDataType DataType;

};

}

#endif