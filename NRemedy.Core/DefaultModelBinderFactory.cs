//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-04-11
//------------------------------------------------------------------
using System;

namespace NRemedy
{
    public static class DefaultFactory
    {
        public static IModelBinder<T> CreateModelBinder<T>()
        {
            return new ModelBinder<T>();
 
        }
    }
}
