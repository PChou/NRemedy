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
    /// 标记Model中的属性对应AR的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ARFieldAttribute : Attribute
    {
        public ARFieldAttribute(uint databaseId, string databaseName, ARType dataType)
        {
            DatabaseID = databaseId;
            DatabaseName = databaseName;
            DataType = dataType;            
            BinderAccess = ModelBinderAccessLevel.OnlyBind | ModelBinderAccessLevel.OnlyUnBind;
        }


        public ARFieldAttribute(){
            BinderAccess = ModelBinderAccessLevel.OnlyBind | ModelBinderAccessLevel.OnlyUnBind;
        }

        /// <summary>
        /// AR字段的数据库ID
        /// </summary>
        public uint DatabaseID { get; set; }

        /// <summary>
        /// AR字段的数据库名，AR的字段名不能为中文
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// AR字段的类型
        /// </summary>
        public ARType DataType { get; set; }

        /// <summary>
        /// 绑定模式，用于处理底层一些数据绑定
        /// </summary>
        public ModelBinderAccessLevel BinderAccess { get; set; }
    }
}
