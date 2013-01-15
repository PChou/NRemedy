#ifndef _AR_ATTACHMENT_H_
#define _AR_ATTACHMENT_H_


using namespace System;


namespace ARNative {

#pragma once

public ref class ARAttachment
{
public:
	ARAttachment(void);
public:
	property String^ AttchmentName;
	property array<Byte>^ Buff;

};


}

#endif