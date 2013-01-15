//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-04-17
//------------------------------------------------------------------
using System;

namespace NRemedy
{
    [Flags]
    public enum ModelBinderAccessLevel
    {
        OnlyBind = 0x01,
        OnlyUnBind = 0x02
    }
}
