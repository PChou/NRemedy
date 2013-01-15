#include "../h/ARGroupByStatictisc.h"

namespace ARNative{

ARGroupByStatictisc::ARGroupByStatictisc(void)
{
}

ARGroupByStatictisc^ ARGroupByStatictisc::ConstructARStatictisc(const ARStatisticsResultStruct* stat)
{
	if(stat == NULL)
		return nullptr;
	ARGroupByStatictisc^ statis = gcnew ARGroupByStatictisc();
	statis->Statictisc = ARValueStructToObj(stat->result);
	if(stat->groupByValues.numItems > 0)
		statis->GroupByValues = gcnew List<Object^>();
	
	for ( int i = 0 ; i < stat->groupByValues.numItems ; i++ ){
		Object^ value = ARValueStructToObj(stat->groupByValues.valueList[i]);
		statis->GroupByValues->Add(value);
	}

	return statis;
}

}
