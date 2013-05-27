#ifndef _AR_Field_Limit_
#define _AR_Field_Limit_


using namespace System::Collections::Generic;
using namespace System;

#pragma once
namespace ARNative {

public ref class AREnum
{
public:
	AREnum(void);
public:
	property unsigned long itemNumber;
	property String^ itemName;
};

public ref class AREnumList
{
public:
	AREnumList(void);
public:
	property List<AREnum^>^ enumList;
};

public ref class ARFieldLimit
{
public:
	ARFieldLimit(void);
public:
	property AREnumList^ enumlimit;
};





}
#endif