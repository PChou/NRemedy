#include "../h/ARServerInfo.h"

namespace ARNative {

ARServerInfo^ ARServerInfo::ConstructARServerInfo(const ARServerInfoStruct* entry)
{
	if(entry == NULL)
		return nullptr;

	ARServerInfo^ serverInfo = gcnew ARServerInfo();
	serverInfo->Type = (ServerInfoType)entry->operation;
	serverInfo->Value = ARValueStructToObj(entry->value);

	return serverInfo;
}

}