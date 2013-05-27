#ifndef _AR_FORM_H_
#define _AR_FORM_H_

#include "../../api/include/ar.h"

using namespace System::Collections::Generic;
using namespace System;

#pragma once
namespace ARNative {

public ref class ARForm
{
public:
	ARForm(void);
public:
	property String^ formName;
	property UInt32 formtype;
	property List<unsigned long>^ assignedGroupList;
	property List<unsigned long>^ groupList;
	property List<unsigned long>^ admingrpList;
	property List<unsigned long>^ getListFields;
	property List<unsigned long>^ sortList;
	property UInt32 auditInfo;
	property String^ defaultVui;
	property unsigned long timestamp;
	property String^ owner;
	property String^ lastChanged;
};
}
#endif