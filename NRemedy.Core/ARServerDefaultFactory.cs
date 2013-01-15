//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------
using System;

namespace NRemedy
{
    public class ARServerDefaultFactory : IARServerFactory
    {
        public virtual IARServer CreateARServer()
        {
            return new ARServer();
        }
    }
}
