#include "../h/ARField.h"

namespace ARNative {

ARField::ARField(void)
{
	assignedGroupList = gcnew List<unsigned long>();
	permissions = gcnew List<unsigned long>();
}

}