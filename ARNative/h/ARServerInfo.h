#ifndef _AR_SERVER_INFO_H_
#define _AR_SERVER_INFO_H_

#include "Util.h"
#include "ARException.h"

using namespace System;
using namespace System::Collections::Generic;

namespace ARNative{

#pragma once
public ref class ARServerInfo
{
public:
	ARServerInfo(void){}
	static ARServerInfo^ ConstructARServerInfo(const ARServerInfoStruct* entry);
public:
	property ServerInfoType Type;
	property Object^ Value;
};


}

#endif