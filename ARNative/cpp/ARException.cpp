#include "../h/ARException.h"

namespace ARNative {

ARException^ ARException::ConstructARException(const ARStatusList* statusList)
{
	ARStatusStruct* pStatus = NULL;
	if(statusList->numItems > 0){
		pStatus = statusList->statusList;
	}
	if(pStatus != NULL){
		ARException^ lastException = nullptr;
		for(int i = 0 ; i < statusList->numItems ; i++,pStatus++)
		{
			lastException = ConstructARExceptionInternal(pStatus,lastException);
		}
		return lastException;
	}
	else
	{
		//return gcnew ARException();
		return nullptr;
	}
}

ARException^ ARException::ConstructARExceptionInternal(const ARStatusStruct* status,ARException^ innerException)
{
	if(status == NULL)
		return nullptr;
	ARException^ r = gcnew ARException(MarshalCharCopyToString(status->messageText),innerException);
	r->messageAppend = MarshalCharCopyToString(status->appendedText);
	r->MessageNumber = status->messageNum;
	r->MessageType = status->messageType;
	return r;
}


}