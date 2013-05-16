#ifndef _AR_GROUP_INFO_H_
#define _AR_GROUP_INFO_H_


#include "Util.h"
#include "../../api/include/ar.h"

using namespace System;


namespace ARNative {

#pragma once

public ref class ARGroupInfo
{
public:
	ARGroupInfo(void);
public:
	property UInt64 GroupId;
	property GroupType GroupType;
	property String^ GroupName;
	property GroupCategory GroupCategory;
	property UInt64 ParentGroupId; 

	static ARGroupInfo^ ConstructARGroupInfo(ARGroupInfoStruct* group);
};



}

#endif