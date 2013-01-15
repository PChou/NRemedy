//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using BMC.ARSystem;
//using System.Reflection;
//using NRemedy;

//namespace NRemedy.Linq
//{
//    public class ModelBinder4Linq<T> : ModelBinder<T>, IModelBinder<T>
//    {
//        public FieldValueList UnBind(T Model)
//        {
//            throw new NotImplementedException();
//        }

//        public FieldValueList UnBindForUpdate(T Model)
//        {
//            throw new NotImplementedException();
//        }

//        public FieldValueList UnBind(T Model, PropertyFilterDelegate filter)
//        {
//            throw new NotImplementedException();
//        }

//        public T Bind(FieldValueList valueList)
//        {
//            if (valueList == null) throw new ArgumentNullException("valueList");
//            T model = Activator.CreateInstance<T>();
//            foreach (PropertyInfo prop in typeof(T).GetProperties
//                //binder need at least writeable property
//                (BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance))
//            {
//                var fieldAttribute = GetARAttributeField(prop, ModelBinderAccessLevel.OnlyBind);
//                if (fieldAttribute == null) continue;
//                //if valueList can not cover model, throw exception
//                //if (!valueList.ContainsKey(fieldAttribute.DatabaseID))
//                //    throw new InvalidCastException(string.Format("valueList cannot cast to model : {0}", typeof(T).FullName),
//                //        new Exception(string.Format("DatabaseID : {0} in model : {1} cannot be found in valueList", fieldAttribute.DatabaseID, typeof(T).FullName)));
//                object value = valueList[fieldAttribute.DatabaseID];
//                if (value == null) continue;
//                //int arType = valueList.GetValueType(fieldAttribute.DatabaseID);
//                prop.SetValue(model, ARConvertToNET(prop.PropertyType, fieldAttribute.DataType, value), null);

//            }

//            return model;
//        }
//    }
//}
