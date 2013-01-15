//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace NRemedy
{
    /// <summary>
    /// 所有Model的基类，定义并实现了属性的OnPropertyChanged，以实现UpdateEntry功能
    /// </summary>
    public class ARBaseForm
    {
        public ARBaseForm()
        {
            ChangedProperties = new Dictionary<string, bool>();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if(ChangedProperties.ContainsKey(propertyName))
                ChangedProperties[propertyName] = true;
            else
                ChangedProperties[propertyName] = false;
        }

        public Dictionary<string, bool> ChangedProperties;

        //add by parkerz
        //since statictisc function has been impelement in the new version
        public object Statictisc;
    }
}
