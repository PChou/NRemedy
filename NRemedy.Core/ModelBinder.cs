//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using ARNative;
using BMC.ARSystem;

namespace NRemedy
{
    /// <summary>
    /// ModelBinder is responsed bind the raw fieldvalueList into T
    /// and unbind the T down to fieldvaluelist
    /// </summary>
    /// <typeparam name="T">Model Type</typeparam>
    public class ModelBinder<T> : IModelBinder<T>
    {
        public List<ARFieldValue> UnBindForUpdate(T Model)
        {
            return UnBind(Model, new PropertyFilterDelegate(PropertyFilterForChanged));
        }

        public List<ARFieldValue> UnBind(T Model)
        {
            return UnBind(Model, null);
        }

        public List<ARFieldValue> UnBind(T Model, PropertyFilterDelegate filter)
        {
            if (Model == null)
                throw new ArgumentNullException("Model");
            List<ARFieldValue> list = new List<ARFieldValue>();
            List<uint> hasAdded = new List<uint>();            
            foreach (PropertyInfo prop in typeof(T).GetProperties
                //unbinder need at least readable property
                (BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldAttribute = GetARAttributeField(prop, ModelBinderAccessLevel.OnlyUnBind);
                if (fieldAttribute == null) continue;
                //if filter supplied,check if being filter
                if (filter != null && filter(Model as ARBaseForm, prop) == false)
                    continue;
                //db id
                uint id = fieldAttribute.DatabaseID;
                if (hasAdded.Contains(id)) throw new DuplicateIDException(id);
                //type
                int typeId = (int)fieldAttribute.DataType;
                object value = prop.GetValue(Model, null);
                list.Add(new ARFieldValue(
                    id,
                    NETConvertToAR(value,fieldAttribute.DataType),
                    //(ARDataType)Enum.ToObject(typeof(ARDataType),fieldAttribute.DataType)
                    (ARDataType)fieldAttribute.DataType
                    ));

                hasAdded.Add(id);
            }
            return list;
        }

        /// <summary>
        /// unbind the model type into fieldId list for GetEntry
        /// </summary>
        /// <returns></returns>
        public List<uint> UnBindToFieldIdList()
        {
            List<uint> Ids = new List<uint>();
            foreach (PropertyInfo prop in typeof(T).GetProperties
                //unbinder need at least readable property
               (BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldAttribute = GetARAttributeField(prop, ModelBinderAccessLevel.OnlyUnBind);
                if (fieldAttribute == null) continue;
                //db id
                uint id = fieldAttribute.DatabaseID;
                if (Ids.Contains(id)) throw new DuplicateIDException(id);
                Ids.Add(id);
            }
            return Ids;
        }

        public T Bind(List<ARFieldValue> valueList)
        {
            if (valueList == null) return default(T);
            //store valueList in dic
            Dictionary<uint, ARFieldValue> dic = new Dictionary<uint, ARFieldValue>();
            foreach (var arfv in valueList){
                dic.Add(arfv.FieldId, arfv);
            }


            T model = Activator.CreateInstance<T>();
            foreach (PropertyInfo prop in typeof(T).GetProperties
                //binder need at least writeable property
                (BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldAttribute = GetARAttributeField(prop, ModelBinderAccessLevel.OnlyBind);
                if (fieldAttribute == null) continue;
                //if valueList can not cover model, throw exception
                
                //if(!valueList.ContainsKey(fieldAttribute.DatabaseID))
                //    throw new InvalidCastException(string.Format("valueList cannot cast to model : {0}",typeof(T).FullName),
                //        new Exception(string.Format("DatabaseID : {0} in model : {1} cannot be found in valueList",fieldAttribute.DatabaseID,typeof(T).FullName)));

                //if valueList can not cover model, continue
                if (!dic.ContainsKey(fieldAttribute.DatabaseID))
                    continue;


                object value = dic[fieldAttribute.DatabaseID].Value;
                int arType = (int)dic[fieldAttribute.DatabaseID].DataType;
                prop.SetValue(model, ARConvertToNET(prop.PropertyType,fieldAttribute.DataType,value) , null);

            }
            
            return model;
        }

        /// <summary>
        /// convert .net value to ar value while unbind and
        /// this function is garantee the type is match        
        /// </summary>
        /// <param name="propValue">.net value</param>
        /// <param name="type">ar type</param>
        /// <returns>ar value</returns>
        public object NETConvertToAR(object propValue, ARType type)
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
                    if (valueType == typeof(String)){
                        return propValue;
                    }
                    break;
                    //TODO:
                case ARType.DiaryField:
                    return (String)propValue;
                case ARType.SelectionField:
                    if (valueType.IsEnum || 
                        valueType == typeof(Int32) || 
                        valueType == typeof(UInt32)){
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
                    if(valueType == typeof(ARAttachment)){
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
        public object ARConvertToNET(Type NETType, ARType ARType ,object ARValue)
        {
            if (NETType == null)
                throw new ArgumentNullException("NETType");
            if (ARValue == null || ARValue is System.DBNull){
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
                    if(nettypeStr == typeof(Int32).FullName)
                        return (Int32)ARValue;
                    break;
                case ARType.RealField:
                    if(nettypeStr == typeof(Double).FullName)
                        return (Double)ARValue;
                    break;
                case ARType.CharacterField:
                    if(ARValue is String)
                        return (String)ARValue;
                    break;
                case ARType.SelectionField:
                    if(NETType.IsEnum || nettypeStr == typeof(Int32).FullName || nettypeStr == typeof(UInt32).FullName)
                        return Enum.ToObject(NETType,ARValue);    //int32 cast to enum
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

        public ARFieldAttribute GetARAttributeField(PropertyInfo pi,ModelBinderAccessLevel accessLevel)
        {
            if (pi == null)
                throw new ArgumentNullException("pi");
            var attrs = pi.GetCustomAttributes(typeof(ARFieldAttribute), false);
            if (attrs.Length > 1)
                throw new CustomAttributeFormatException(
                    string.Format("Mutiple ARFieldAttribute is found on Property : {0}.", pi.Name));
            if (attrs.Length == 0) return null;
            ARFieldAttribute attribute = attrs[0] as ARFieldAttribute;
            if (attribute.DatabaseID == 0)
                throw new CustomAttributeFormatException(
                    string.Format("DatabaseID of ARFieldAttribute on Property : {0} is missing.", pi.Name));
            if (attribute.DataType == ARType.None)
                throw new CustomAttributeFormatException(
                    string.Format("DataType of ARFieldAttribute on Property : {0} cannot be null.", pi.Name));
            if ((attribute.BinderAccess & accessLevel) == accessLevel)
                return attribute;
            else
                return null;
        }

        /// <returns>true : pass; false : block</returns>
        private bool PropertyFilterForChanged(ARBaseForm model, PropertyInfo pi)
        {
            if (model == null)
                throw new ArgumentNullException("model", "model is probably not inherit from ARBaseForm");
            if (pi == null)
                throw new ArgumentNullException("pi");
            if (model.ChangedProperties.ContainsKey(pi.Name))
                return model.ChangedProperties[pi.Name];
            else
                return false;
        }
    }
}
