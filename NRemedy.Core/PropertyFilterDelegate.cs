//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-04-17
//------------------------------------------------------------------
using System;
using System.Reflection;

namespace NRemedy
{
    /// <summary>
    /// filter property
    /// </summary>
    /// <param name="model">model object</param>
    /// <param name="pi">property info</param>
    /// <returns>true : pass; false : block</returns>
    public delegate bool PropertyFilterDelegate(ARBaseForm model,PropertyInfo pi);

    public delegate bool PropertyFilterDelegate2(PropertyInfo pi);
}
