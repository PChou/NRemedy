//these funciton is added for free AR struct
//you may want to ask me why need this since FreeARXXXX is supplied by AR arfree.h
//explain blow:
//AR core c dymanic dll is depends msvcrt71.dll which is very old and vs2012 support vc90 lowest
//so the process may have two version msvcrt in memeory which will cause two heap maintained by two different version c runtime lib
//but memory free could not operate cross different c runtime heap in debug mode since _CrtIsValidHeapPointer
//so the workaround is the _Free.h which is used to free AR struct for dll not based on msvcrt71

#ifndef __FREE_H_
#define __FREE_H_

#include "../../api/include/ar.h"
#include "../../api/include/arerrno.h"
#include "../../api/include/arextern.h"
#include "../../api/include/arstruct.h"

#include <assert.h>

#include <yvals.h> // warning numbers get enabled in yvals.h

#pragma warning(disable:4018)

//Not needed impelement because ARStatusList is always malloc memeory by msvcrt71
extern void _FreeARStatusList(ARStatusList *, ARBoolean);

//!!important:this function _FreeARFieldValueList use the struct ARValueStruct which using union in struct, so int and point are both use same memory
//address, the only indication on the union is ARValueStruct.datatype, must garantee they will not mismatch ,ohter wise will cause
//memoery exception
extern void _FreeARFieldValueList(ARFieldValueList *, ARBoolean);
extern void _FreeAREntryIdList(AREntryIdList* , ARBoolean);
extern void _FreeARInternalIdList(ARInternalIdList* , ARBoolean);

//Not needed impelement because ARStatisticsResultList is always malloc memeory by msvcrt71
extern void _FreeARStatisticsResultList(ARStatisticsResultList* , ARBoolean);

//Not needed impelement because ARQualifierStruct is always malloc memeory by msvcrt71
extern void _FreeARQualifierStruct(ARQualifierStruct* , ARBoolean);

//!!!import: this function only implement the situation that tag == AR_FIELD
//and in the case, the struct has no heap memeory alloc
//so only assert(s->tag == AR_FIELD); in the function body
extern void _FreeARFieldValueOrArithStruct(ARFieldValueOrArithStruct* , ARBoolean);

extern void _FreeAREntryListFieldList(AREntryListFieldList* , ARBoolean);
extern void _FreeARSortList(ARSortList* , ARBoolean);

//Not needed impelement because AREntryListFieldValueList is always malloc memeory by msvcrt71
extern void _FreeAREntryListFieldValueList(AREntryListFieldValueList* , ARBoolean);

extern void _FreeARLocStruct(ARLocStruct* , ARBoolean);

#endif