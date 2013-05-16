#include "../h/ARGroupInfo.h"

namespace ARNative {

ARGroupInfo::ARGroupInfo(void)
{
}


ARGroupInfo^ ARGroupInfo::ConstructARGroupInfo(ARGroupInfoStruct* group)
{
	if(group == NULL)
		return nullptr;
	ARGroupInfo^ groupinfo = gcnew ARGroupInfo();
	groupinfo->GroupId = group->groupId;
	groupinfo->GroupCategory = (ARNative::GroupCategory)(group->groupCategory);
	groupinfo->GroupType = (ARNative::GroupType)(group->groupType);
	groupinfo->ParentGroupId = group->groupParent;
	//only return the first group name
	//since one groupid can be mapped to multiple groupname
	//but actually it is not very useful
	groupinfo->GroupName = MarshalCharCopyToString(group->groupName.nameList[0]);
	return groupinfo;
}

}