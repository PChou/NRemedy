using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ARNative;

namespace NRemedy
{
    public class PropertyAndField<TModel>
    {
        public PropertyInfo Property { get; set; }

        public uint DatabaseId { get; set; }

        //no need
        public string DatabaseName { get; set; }

        public ARType DatabaseType { get; set; }

        public ModelBinderAccessLevel AccessLevel { get; set; }

        public Func<TModel, object> Getter { get; set; }

        public Action<TModel, object> Setter { get; set; }

        public object GetValue(TModel model)
        {
            if (Getter == null)
                return Property.GetValue(model, null);
            return Getter(model);
        }

        public void SetValue(TModel model, object value)
        {
            if (Getter == null)
                Property.SetValue(model, value, null);
            else
                Setter(model,value);
        }

        public object GetValueC(TModel model)
        {
            object origin = null;
            if (Getter == null)
                origin = Property.GetValue(model, null);
            else
                origin = Getter(model);
            return NETConvertToAR(origin, DatabaseType);
        }

        public void SetValueC(TModel model, object value)
        {
            object c_value = ARConvertToNET(Property.PropertyType, DatabaseType, value);

            if (Setter == null)
                Property.SetValue(model, c_value, null);
            else
                Setter(model, c_value);
        }


        /// <summary>
        /// convert .net value to ar value while unbind and
        /// this function is garantee the type is match        
        /// </summary>
        /// <param name="propValue">.net value</param>
        /// <param name="type">ar type</param>
        /// <returns>ar value</returns>
        public static object NETConvertToAR(object propValue, ARType type)
        {
            if (type == ARType.None)
                throw new ArgumentOutOfRangeException("type");
            if (propValue == null)
                return null;

            Type valueType = propValue.GetType();
            switch (type)
            {
                case ARType.IntegerField:
                    if (valueType == typeof(Nullable<Int32>) || valueType == typeof(Int32))
                    {
                        return propValue;
                    }
                    break;
                case ARType.RealField:
                    if (valueType == typeof(Nullable<Double>) || valueType == typeof(Double))
                    {
                        return propValue;
                    }
                    break;
                case ARType.CharacterField:
                    if (valueType == typeof(String))
                    {
                        return propValue;
                    }
                    break;
                //TODO:
                case ARType.DiaryField:
                    return (String)propValue;
                case ARType.SelectionField:
                    if (valueType.IsEnum ||
                        valueType == typeof(Int32) ||
                        valueType == typeof(UInt32))
                    {
                        return propValue;
                    }
                    break;
                case ARType.DateTimeField:
                    if (valueType == typeof(Nullable<DateTime>) || valueType == typeof(DateTime))
                    {
                        TimeSpan ts = ((DateTime)propValue).ToUniversalTime() - new DateTime(1970, 1, 1);
                        return Convert.ToInt32(ts.TotalSeconds);
                    }
                    break;
                case ARType.DecimalField:
                    if (valueType == typeof(Decimal) || valueType == typeof(Decimal))
                    {
                        return propValue;
                    }
                    break;
                //actually these below has not been impelement
                case ARType.AttachmentField:
                    if (valueType == typeof(ARAttachment))
                    {
                        return propValue;
                    }
                    break;
                case ARType.CurrencyField:
                    break;
                case ARType.DateOnlyField:
                    break;
                case ARType.TimeOnlyField:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            throw new InvalidCastException(string.Format("propValue : {0} cannot cast to the ARType : {1}", propValue, type));
        }

        /// <summary>
        /// convert ar value to .net value
        /// </summary>
        /// <param name="NETType"></param>
        /// <param name="ARType"></param>
        /// <param name="ARValue"></param>
        /// <returns>.net value</returns>
        public static object ARConvertToNET(Type NETType, ARType ARType, object ARValue)
        {
            if (NETType == null)
                throw new ArgumentNullException("NETType");
            if (ARValue == null || ARValue is System.DBNull)
            {
                if (NETType.IsValueType) return default(ValueType);
                return null;
            }
            if (NETType.IsGenericType && NETType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                NETType = NETType.GetGenericArguments()[0];
            }
            string nettypeStr = NETType.FullName;

            switch (ARType)
            {
                case ARType.IntegerField:
                    if (nettypeStr == typeof(Int32).FullName)
                        return (Int32)ARValue;
                    break;
                case ARType.RealField:
                    if (nettypeStr == typeof(Double).FullName)
                        return (Double)ARValue;
                    break;
                case ARType.CharacterField:
                    if (ARValue is String)
                        return (String)ARValue;
                    break;
                case ARType.SelectionField:
                    if (NETType.IsEnum || nettypeStr == typeof(Int32).FullName || nettypeStr == typeof(UInt32).FullName)
                        return Enum.ToObject(NETType, ARValue);    //int32 cast to enum
                    break;
                case ARType.DateTimeField:
                    //arvalue will be seconds from 1970-1-1
                    long seconds = Convert.ToInt64(ARValue);
                    DateTime dt = new DateTime(1970, 1, 1) + new TimeSpan(seconds * 10000000);
                    return dt.ToLocalTime();
                case ARType.DecimalField:
                    if (nettypeStr == typeof(Decimal).FullName)
                        return Convert.ToDecimal(ARValue);
                    break;
                //these has not been impelement yet
                case ARType.DiaryField:
                    break;
                case ARType.AttachmentField:
                    if (nettypeStr == typeof(ARAttachment).FullName)
                        return (ARAttachment)ARValue;
                    break;
                case ARType.CurrencyField:
                    break;
                case ARType.DateOnlyField:
                    break;
                case ARType.TimeOnlyField:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            throw new InvalidCastException(string.Format("ARValue : {0} with type of {1} cannot cast to the NETType : {2}", ARValue, ARType, NETType));
        }
    }
}
