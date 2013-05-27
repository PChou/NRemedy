#ifndef _AR_Field_H_
#define _AR_Field_H_

#include "ARFieldLimit.h"
#include "ARFieldValue.h"

using namespace System::Collections::Generic;
using namespace System;

#pragma once
namespace ARNative {

public ref class ARField
{
public:
	ARField(void);
public:
	property unsigned long fieldId;
	property String^ fieldName;
	property ARDataType dataType;
	property UInt32 option;
	property UInt32 createMode;
	property UInt32 fieldOption;
	property ARFieldValue^ defaultVal;
	property List<unsigned long>^ assignedGroupList;
	property List<unsigned long>^ permissions;
	property long timestamp;
	property String^ owner;
	property String^ lastChanged;
	property ARFieldLimit^ limit;
	property bool isenum;
};
}
#endif