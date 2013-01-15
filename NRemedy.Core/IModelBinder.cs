//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-04-11
//------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using ARNative;

namespace NRemedy
{
    public interface IModelBinder<T>
    {
        List<ARFieldValue> UnBind(T Model);
        List<ARFieldValue> UnBindForUpdate(T Model);
        List<ARFieldValue> UnBind(T Model, PropertyFilterDelegate filter);
        List<UInt32> UnBindToFieldIdList();
        T Bind(List<ARFieldValue> valueList);
        ARFieldAttribute GetARAttributeField(PropertyInfo pi, ModelBinderAccessLevel accessLevel);
    }
}
