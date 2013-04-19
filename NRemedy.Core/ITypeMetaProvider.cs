using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NRemedy
{
    public interface ITypeMetaProvider<TModel>
    {
        //create|delete|set
        string GetFormNameFromModelType();
        //create
        IEnumerable<PropertyAndField<TModel>> GetPropertyInfoes(BindingFlags bindFlags, PropertyFilterDelegate2 filter);
        //set
        IEnumerable<PropertyAndField<TModel>> GetPropertyInfoes(ARBaseForm Model, BindingFlags bindFlags, PropertyFilterDelegate filter);
        //delete
        PropertyAndField<TModel> GetEntryIdPropertyInfo();
    }
}
