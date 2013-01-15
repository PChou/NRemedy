#include "../h/ARFieldValue.h"

namespace ARNative {

ARFieldValue::ARFieldValue(void)
{
	
}


ARFieldValue::ARFieldValue(UInt32 fieldId,Object^ value,ARDataType dataType)
{
	FieldId = fieldId;
	Value = value;
	DataType = dataType;
}

ARFieldValue^ ARFieldValue::ConstructARFieldValue(const ARFieldValueStruct* fvs)
{
	if(fvs == NULL)
		return nullptr;
	ARFieldValue^ arFieldValue = gcnew ARFieldValue();
	arFieldValue->FieldId = fvs->fieldId;
	arFieldValue->DataType = (ARNative::ARDataType)fvs->value.dataType;
	arFieldValue->Value = ARValueStructToObj(fvs->value);
	return arFieldValue;
}

void ARFieldValue::ConstructARFieldValueStruct(ARFieldValueStruct* des , ARFieldValue^ src)
{
	if(des == NULL || src == nullptr)
		return;
	des->fieldId = src->FieldId;
	ObjToARValueStruct(des->value,src->Value,src->DataType);
}


}