#include "../h/Util.h"
//#include "IntelligentARStruct.h"


namespace ARNative {
//copy String to char safely
char* MarshalStringCopyToChar(String^ Source)
{
	//std::string tmp = marshal_as<std::string>(Source);
	//strcpy(Des,tmp.c_str());
	if(String::IsNullOrEmpty(Source))
		return NULL;
	array<Byte>^ vText = System::Text::Encoding::UTF8->GetBytes(Source);
	pin_ptr<unsigned char> pText = &vText[0];
	char* Des = (char*)calloc(vText->Length+1,sizeof(char));
	memcpy(Des, pText, vText->Length);
	Des[vText->Length] = '\0';
	return Des;
}

void MarshalStringCopyToCharStack(char* Des,String^ Source)
{
	if(String::IsNullOrEmpty(Source))
		return;
	array<Byte>^ vText = System::Text::Encoding::UTF8->GetBytes(Source);
	pin_ptr<unsigned char> pText = &vText[0];
	memcpy(Des, pText, vText->Length);
	Des[vText->Length] = '\0';
}

//convert to String^ from char* with '\0' termination
String^ MarshalCharCopyToString(const char* Source)
{
	//return marshal_as<String^>(Source);
	if(!Source)
		return String::Empty;

	int nLen = strlen(Source);
	if(nLen == 0)
		return String::Empty;

	array<Byte>^ buffer = gcnew array<Byte>(nLen);

	for(int j=0; j<nLen; j++)
		buffer[j] = Source[j];	

	return System::Text::Encoding::UTF8->GetString(buffer);
}

Object^ ARValueStructToObj(const ARValueStruct& arValueStruct)
{
	switch (arValueStruct.dataType)
	{
		case AR_DATA_TYPE_NULL           : /* code for a NULL value */
			return nullptr;
		case AR_DATA_TYPE_KEYWORD        : /* code indicating a keyword setting */
			return arValueStruct.u.keyNum;
		case AR_DATA_TYPE_INTEGER        : /* codes for the data type of a value */
			return arValueStruct.u.intVal;
		case AR_DATA_TYPE_REAL           :
			return arValueStruct.u.realVal;
		case AR_DATA_TYPE_ENUM           :
			return arValueStruct.u.enumVal;
		case AR_DATA_TYPE_TIME           :
			return arValueStruct.u.timeVal;
		case AR_DATA_TYPE_BITMASK        :
			return arValueStruct.u.intVal;
		case AR_DATA_TYPE_ATTACH        :
			{
				ARAttachment^ att = gcnew ARAttachment();
				att->AttchmentName = MarshalCharCopyToString(arValueStruct.u.attachVal->name);
				unsigned char * buffer_point = arValueStruct.u.attachVal->loc.u.buf.buffer;
				ARULong32 nlen = arValueStruct.u.attachVal->loc.u.buf.bufSize;
				att->Buff = gcnew array<Byte>(nlen);

				for(int j=0; j < nlen; j++)
					att->Buff[j] = buffer_point[j];

				return att;
			}
		case AR_DATA_TYPE_BYTES          :
		case AR_DATA_TYPE_CURRENCY      :
			break;
		case AR_DATA_TYPE_DATE          :
			return arValueStruct.u.dateVal;
		case AR_DATA_TYPE_TIME_OF_DAY   :
			return arValueStruct.u.timeOfDayVal;
		case AR_DATA_TYPE_CHAR           :
			return MarshalCharCopyToString(arValueStruct.u.charVal);
		case AR_DATA_TYPE_DIARY          :
			return MarshalCharCopyToString(arValueStruct.u.diaryVal);
		case AR_DATA_TYPE_DECIMAL       :
			return MarshalCharCopyToString(arValueStruct.u.decimalVal);
		default:
			break;
	}
	return nullptr;
}

void ObjToARValueStruct(ARValueStruct &des,Object^ src,ARDataType dataType)
{
	if(src == nullptr){
		//if src is null, change the data type to null
		des.dataType = AR_DATA_TYPE_NULL;
		des.u.charVal = NULL;
		return;
	}
	des.dataType = (unsigned int)dataType;
	switch (dataType){
		case ARDataType::DATA_TYPE_NULL:            /* code for a NULL value */
			des.u.ptrVal = NULL;
			break;
		case ARDataType::DATA_TYPE_KEYWORD:         /* code indicating a keyword setting */
		case ARDataType::DATA_TYPE_BITMASK:
			//TODO
			des.u.keyNum = (UInt32)src;
			break;
		case ARDataType::DATA_TYPE_INTEGER:         /* codes for the data type of a value */
		case ARDataType::DATA_TYPE_ENUM: 
		case ARDataType::DATA_TYPE_TIME:
		case ARDataType::DATA_TYPE_DATE:         
		case ARDataType::DATA_TYPE_TIME_OF_DAY: 
			des.u.intVal = (Int32)src;
			break;
		case ARDataType::DATA_TYPE_REAL:
			des.u.realVal = (Double)src;
			break;
		case ARDataType::DATA_TYPE_CHAR:
		case ARDataType::DATA_TYPE_DIARY:
		case ARDataType::DATA_TYPE_DECIMAL:
			{
				String^ value = src->ToString();
				des.u.charVal = MarshalStringCopyToChar(value);
				break;
			}
		case ARDataType::DATA_TYPE_ATTACH:    
			{
				ARAttachment^ att = (ARAttachment^)src;
				des.u.attachVal = (ARAttachStruct*)calloc(1,sizeof(ARAttachStruct));
				des.u.attachVal->name = MarshalStringCopyToChar(att->AttchmentName);
				des.u.attachVal->loc.locType = AR_LOC_BUFFER;
				ARLong32 nLen = att->Buff->Length;
				des.u.attachVal->loc.u.buf.bufSize = nLen;
				des.u.attachVal->loc.u.buf.buffer = (unsigned char*)calloc(nLen,sizeof(unsigned char));
				array<Byte>^ tBuf =  att->Buff;
				pin_ptr<unsigned char> pBuff = &tBuf[0];
				memcpy(des.u.attachVal->loc.u.buf.buffer, pBuff, nLen);
				break;
			}
		case ARDataType::DATA_TYPE_BYTES:
		case ARDataType::DATA_TYPE_CURRENCY: 
			break;
		default:
			break;
	}
}

}