
using System;
using System.Collections.Generic;
using ARNative;

namespace NRemedy
{
    public interface IARSchema
    {
        List<ARForm> GetListFormWithDetail();
        List<ARForm> GetListFormWithDetail(ARFormFilterDelegate formFilter);
        List<ARField> GetListFieldWithDetail(string formName);
        List<ARField> GetListFieldWithDetail(string formName, ARFieldFilterDelegate fieldFilter);
    }
}
