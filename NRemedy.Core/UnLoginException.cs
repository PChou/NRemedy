//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------
using System;

namespace NRemedy
{
    public class UnLoginException : Exception
    {
        public override string Message
        {
            get
            {
                return "Action can not be performed because of invalid login context.";
            }
        }
    }
}
