#ifndef _AR_EXCEPTION_H_
#define _AR_EXCEPTION_H_

#include "../../api/include/ar.h"
#include "Util.h"

using namespace System;

#pragma once

namespace ARNative {
public ref class ARException : public Exception
{
public:
	ARException(void){}
	ARException(String^ Message,Exception^ InnerException) : Exception(Message,InnerException){}
	~ARException(void){}
	static ARException^ ConstructARException(const ARStatusList* statusList);
private:
	static ARException^ ConstructARExceptionInternal(const ARStatusStruct* status,ARException^ innerException);
public:
	//property String^ Message; //inherit from Exception
	property String^ MessageAppend;
	property Int32 MessageNumber;
	property UInt32 MessageType;
private:
	String^ messageAppend;
};


}
#endif