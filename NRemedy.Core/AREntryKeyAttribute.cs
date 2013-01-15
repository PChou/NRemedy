//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------
using System;

namespace NRemedy
{
    /// <summary>
    /// 设置Model的主键，对生成的Model，必须手动对一个Property标记该Attribute才能使用UpdateEntry功能
    /// 否则将报Can not find data catelog(100)错
    /// </summary>
    public class AREntryKeyAttribute : Attribute
    {

    }
}
