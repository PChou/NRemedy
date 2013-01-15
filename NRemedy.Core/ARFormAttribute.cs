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
    /// 标记Model对应AR的哪个表单
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ARFormAttribute : Attribute
    {
        public ARFormAttribute(string formName)
        {
            FormName = formName;
        }

        public ARFormAttribute()
        {
        }

        /// <summary>
        /// 表单名
        /// </summary>
        public string FormName { get; set; }
    }
}
