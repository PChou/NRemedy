//------------------------------------------------------------------
// System Name:    NRemedy
// Component:      NRemedy
// Create by:      Parker Zhou (parkerz@wicresoft.com)
// Create Date:    2012-12-27
//------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARNative;
using System.Linq.Expressions;
using System.Reflection;

namespace NRemedy.Linq
{
    /// <summary>
    /// ARProxy extension method,argument for Lambda expression
    /// </summary>
    public static class ARProxyLambdaExtension
    {
        public static void DeleteEntryList<TModel>(this ARProxy<TModel> proxy, Expression<Func<TModel, bool>> expression) 
            where TModel : ARBaseForm
        {
            if (expression == null)
                throw new ArgumentNullException("expression must not null. If want to delete all entries, try to use m => true");
            ConditionExpressionVisitor visitor = new ConditionExpressionVisitor();
            var expEvaled = Evaluator.PartialEval(expression);
            ConditionResult tr = visitor.Translate(expEvaled);
            string qu = tr.Qulification == null ? null : tr.Qulification.ToString();

            proxy.DeleteEntryList(qu);
        }

        public static void SetEntry<TModel>(
            this ARProxy<TModel> proxy, TModel model ,params Expression<Func<TModel, object>>[] propertiesTobeUp)
            where TModel : ARBaseForm
        {
            if (propertiesTobeUp == null || propertiesTobeUp.Length == 0)
                return;
            ITypeMetaProvider<TModel> metaProvider = new ExpressionTypeMetaProvider<TModel>(propertiesTobeUp);

            ModelMeteData<TModel> MetaData = new ModelMeteData<TModel>();
            MetaData.FormName = metaProvider.GetFormNameFromModelType();
            var entryIdProp = metaProvider.GetEntryIdPropertyInfo();
            if (entryIdProp == null)
                throw new CustomAttributeFormatException("Can not find EntryId's PropertyInfo.");
            MetaData.EntryId = (string)entryIdProp.GetValue(model);
            MetaData.Model = model;
            var props = metaProvider.GetPropertyInfoes(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null);
            MetaData.Properties = new List<PropertyAndField<TModel>>();

            foreach (var p in props)
            {
                if ((p.AccessLevel & ModelBinderAccessLevel.OnlyUnBind) == ModelBinderAccessLevel.OnlyUnBind)
                {
                    MetaData.Properties.Add(p);
                }
            }

            proxy.SetEntry(MetaData);

        }

        public static void SetEntryList<TModel>(this ARProxy<TModel> proxy, 
            Expression<Func<TModel, bool>> expression, TModel model, params Expression<Func<TModel, object>>[] propertiesTobeUp)
            where TModel : ARBaseForm
        {
            if (expression == null)
                throw new ArgumentNullException("expression must not null. If want to delete all entries, try to use m => true");
            ConditionExpressionVisitor visitor = new ConditionExpressionVisitor();
            var expEvaled = Evaluator.PartialEval(expression);
            ConditionResult tr = visitor.Translate(expEvaled);
            string qu = tr.Qulification == null ? null : tr.Qulification.ToString();

            SetEntryList(proxy, qu, model, propertiesTobeUp);
        }


        public static void SetEntryList<TModel>(
            this ARProxy<TModel> proxy,string qualification ,TModel model, params Expression<Func<TModel, object>>[] propertiesTobeUp)
            where TModel : ARBaseForm
        {
            if (propertiesTobeUp == null || propertiesTobeUp.Length == 0)
                return;
            ITypeMetaProvider<TModel> metaProvider = new ExpressionTypeMetaProvider<TModel>(propertiesTobeUp);

            ModelMeteData<TModel> MetaData = new ModelMeteData<TModel>();
            MetaData.FormName = metaProvider.GetFormNameFromModelType();

            MetaData.Model = model;
            var props = metaProvider.GetPropertyInfoes(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null);
            MetaData.Properties = new List<PropertyAndField<TModel>>();

            foreach (var p in props)
            {
                if ((p.AccessLevel & ModelBinderAccessLevel.OnlyUnBind) == ModelBinderAccessLevel.OnlyUnBind)
                {
                    MetaData.Properties.Add(p);
                }
            }

            int total = -1;
            List<AREntry> entries = proxy.GetARServer().GetEntryList(MetaData.FormName, qualification, null, 0, null, ref total, null);
            
            foreach (var entry in entries)
            {
                string entryid = string.Join("|", entry.EntryIds.ToArray());
                MetaData.EntryId = entryid;
                proxy.SetEntry(MetaData);
            }
        }

        public static TModel GetEntry<TModel>(
            this ARProxy<TModel> proxy, string EntryId, params Expression<Func<TModel, object>>[] propertiesTobeUp)
            where TModel : ARBaseForm
        {
            if (propertiesTobeUp == null || propertiesTobeUp.Length == 0)
                return null;
            ITypeMetaProvider<TModel> metaProvider = new ExpressionTypeMetaProvider<TModel>(propertiesTobeUp);

            ModelMeteData<TModel> MetaData = new ModelMeteData<TModel>();
            MetaData.FormName = metaProvider.GetFormNameFromModelType();
            MetaData.EntryId = EntryId;

            var props = metaProvider.GetPropertyInfoes(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null);
            MetaData.Properties = new List<PropertyAndField<TModel>>();

            foreach (var p in props)
            {
                if ((p.AccessLevel & ModelBinderAccessLevel.OnlyBind) == ModelBinderAccessLevel.OnlyBind)
                {
                    MetaData.Properties.Add(p);
                }
            }

            return proxy.GetEntry(MetaData);
        }

        public static IList<TModel> GetEntryList<TModel>(this ARProxy<TModel> proxy,
           Expression<Func<TModel, bool>> expression,
           uint StartIndex,
           uint? RetrieveCount,
           TotalMatch totalMatch,
           List<ARSortInfo> sortInfo,
           params Expression<Func<TModel, object>>[] propertiesTobeUp)
           where TModel : ARBaseForm
        {
            if (expression == null)
                throw new ArgumentNullException("expression must not null. If want to delete all entries, try to use m => true");
            ConditionExpressionVisitor visitor = new ConditionExpressionVisitor();
            var expEvaled = Evaluator.PartialEval(expression);
            ConditionResult tr = visitor.Translate(expEvaled);
            string qu = tr.Qulification == null ? null : tr.Qulification.ToString();

            return GetEntryList(proxy, qu, StartIndex, RetrieveCount, totalMatch, sortInfo, propertiesTobeUp);
        }


        public static IList<TModel> GetEntryList<TModel>(this ARProxy<TModel> proxy,
            string qualification,
            uint StartIndex,
            uint? RetrieveCount,
            TotalMatch totalMatch,
            List<ARSortInfo> sortInfo,
            params Expression<Func<TModel, object>>[] propertiesTobeUp)
            where TModel : ARBaseForm
        {
            if (propertiesTobeUp == null || propertiesTobeUp.Length == 0)
                return null;
            ITypeMetaProvider<TModel> metaProvider = new ExpressionTypeMetaProvider<TModel>(propertiesTobeUp);

            ModelMeteData<TModel> MetaData = new ModelMeteData<TModel>();
            MetaData.FormName = metaProvider.GetFormNameFromModelType();

            var props = metaProvider.GetPropertyInfoes(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null);
            MetaData.Properties = new List<PropertyAndField<TModel>>();

            foreach (var p in props)
            {
                if ((p.AccessLevel & ModelBinderAccessLevel.OnlyBind) == ModelBinderAccessLevel.OnlyBind)
                {
                    MetaData.Properties.Add(p);
                }
            }

            return proxy.GetEntryList(StartIndex,qualification, MetaData , RetrieveCount, totalMatch, sortInfo);
        }




    }
}
