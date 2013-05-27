#include "../h/ARForm.h"

namespace ARNative {

ARForm::ARForm(void)
{
	assignedGroupList = gcnew List<unsigned long>();
	groupList = gcnew List<unsigned long>();
	admingrpList = gcnew List<unsigned long>();
	getListFields = gcnew List<unsigned long>();
	sortList = gcnew List<unsigned long>();
}

}
