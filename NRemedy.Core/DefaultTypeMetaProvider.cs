using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NRemedy
{
    public class DefaultTypeMetaProvider<TModel> : ITypeMetaProvider<TModel>
    {
        protected ARFieldAttribute GetARAttributeField(PropertyInfo pi)
        {
            if (pi == null)
                throw new ArgumentNullException("pi");
            var attrs = pi.GetCustomAttributes(typeof(ARFieldAttribute), false);
            if (attrs.Length > 1)
                throw new CustomAttributeFormatException(
                    string.Format("Mutiple ARFieldAttribute is found on Property : {0}.", pi.Name));
            if (attrs.Length == 0) return null;
            ARFieldAttribute attribute = attrs[0] as ARFieldAttribute;
            if (attribute.DataType == ARType.None)
                throw new CustomAttributeFormatException(
                    string.Format("DataType of ARFieldAttribute on Property : {0} cannot be null.", pi.Name));
            return attribute;
        }

        public virtual string GetFormNameFromModelType()
        {

            string name = string.Empty;
            var attributes = typeof(TModel).GetCustomAttributes(false);
            ARFormAttribute formAttribute = null;
            foreach (var attr in attributes)
            {
                if (attr is ARFormAttribute)
                {
                    formAttribute = (ARFormAttribute)attr;
                    break;
                }
            }

            if (formAttribute != null)
                name = formAttribute.FormName;

            return name;
        }


        public virtual IEnumerable<PropertyAndField<TModel>> GetPropertyInfoes
            (ARBaseForm Model, BindingFlags bindFlags, PropertyFilterDelegate filter)
        {
            if (Model == null)
                throw new ArgumentNullException("Model");
            Type ModelType = typeof(TModel);
            var properties = ModelType.GetProperties(bindFlags);
            List<PropertyAndField<TModel>> list = new List<PropertyAndField<TModel>>();
            foreach (var p in properties)
            {
                if (filter != null){
                    if (!filter(Model, p)) continue;
                }
                PropertyAndField<TModel> pf = new PropertyAndField<TModel>();
                pf.DatabaseId = GetDatabaseIdFromPropertyInfo(p);
                pf.DatabaseName = GetDatabaseNameFromPropertyInfo(p);
                pf.DatabaseType = GetDataTypeFromPropertyInfo(p);
                pf.AccessLevel = GetAccessLevelFromPropertyInfo(p);
                pf.Property = p;
                //pf.Getter
                //pf.Setter
                list.Add(pf);
            }

            return list;
        }

        public virtual IEnumerable<PropertyAndField<TModel>> GetPropertyInfoes
            (BindingFlags bindFlags, PropertyFilterDelegate2 filter)
        {
            Type ModelType = typeof(TModel);
            var properties = ModelType.GetProperties(bindFlags);
            List<PropertyAndField<TModel>> list = new List<PropertyAndField<TModel>>();
            foreach (var p in properties)
            {
                if (filter != null)
                {
                    if (!filter(p)) continue;
                }
                PropertyAndField<TModel> pf = new PropertyAndField<TModel>();
                pf.DatabaseId = GetDatabaseIdFromPropertyInfo(p);
                pf.DatabaseName = GetDatabaseNameFromPropertyInfo(p);
                pf.DatabaseType = GetDataTypeFromPropertyInfo(p);
                pf.AccessLevel = GetAccessLevelFromPropertyInfo(p);
                pf.Property = p;
                //pf.Getter
                //pf.Setter
                list.Add(pf);
            }

            return list;

        }

        protected virtual uint GetDatabaseIdFromPropertyInfo(PropertyInfo propInfo)
        {
            var attr = GetARAttributeField(propInfo);
            if(attr == null)
                throw new CustomAttributeFormatException();
            return attr.DatabaseID;
        }

        protected virtual string GetDatabaseNameFromPropertyInfo(PropertyInfo propInfo)
        {
            var attr = GetARAttributeField(propInfo);
            if (attr == null)
                throw new CustomAttributeFormatException();
            return attr.DatabaseName;
        }

        protected virtual ARType GetDataTypeFromPropertyInfo(PropertyInfo propInfo)
        {
            var attr = GetARAttributeField(propInfo);
            if (attr == null)
                throw new CustomAttributeFormatException();
            return attr.DataType;
        }

        protected virtual ModelBinderAccessLevel GetAccessLevelFromPropertyInfo(PropertyInfo propInfo)
        {
            var attr = GetARAttributeField(propInfo);
            if (attr == null)
                throw new CustomAttributeFormatException();
            return attr.BinderAccess;
        }


        public virtual PropertyAndField<TModel> GetEntryIdPropertyInfo()
        {
            foreach (PropertyInfo item in typeof(TModel).GetProperties())
            {
                var attributes = item.GetCustomAttributes(false);
                foreach (var attr in attributes)
                {
                    if (attr is AREntryKeyAttribute)
                    {
                        PropertyAndField<TModel> pf = new PropertyAndField<TModel>();
                        pf.DatabaseId = GetDatabaseIdFromPropertyInfo(item);
                        pf.DatabaseName = GetDatabaseNameFromPropertyInfo(item);
                        pf.DatabaseType = GetDataTypeFromPropertyInfo(item);
                        pf.AccessLevel = GetAccessLevelFromPropertyInfo(item);
                        pf.Property = item;
                        //pf.Getter
                        //pf.Setter
                        return pf;
                    }
                }
            }
            return null;
        }

    }
}
