using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Reflection;
using NRemedy;
using ARNative;
using System.Collections;


namespace NRemedy.Linq
{
    public class ARQueryProvider<T>  : QueryProvider
        where T : ARBaseForm
    {
        protected object context;
        //protected object factory;

        //dictionary cache of model and property and id
        //Form
            //property1 100001
            //property2 100002
        protected static Dictionary<string, Dictionary<string, uint>> _Cache;

        public ARQueryProvider(object context)
        {
            this.context = context;
            //this.factory = factory;
            if(_Cache == null)
                _Cache = new Dictionary<string, Dictionary<string, uint>>();
        }

        public TranslateResult ExecuteOnlyTranslate(Expression expression)
        {
            //translate expression
            //visit Evalable expression
            expression = Evaluator.PartialEval(expression);
            //translate
            return this.Translate(expression);
        }

        public override object Execute(Expression expression)
        {
            //translate expression
            //visit Evalable expression
            expression = Evaluator.PartialEval(expression);
            //translate
            TranslateResult tr = this.Translate(expression);

            //T is model of AR,cache the id and property name map
            if(!_Cache.ContainsKey(typeof(T).FullName))
            {
                Dictionary<string,uint> propertry = new Dictionary<string,uint>();
                _Cache.Add(typeof(T).FullName,propertry);
                foreach (PropertyInfo prop in typeof(T).GetProperties
                //unbinder need at least readable property
               (BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    var fieldAttribute = GetARAttributeField(prop, ModelBinderAccessLevel.OnlyUnBind);
                    if (fieldAttribute == null) continue;
                    propertry.Add(prop.Name,fieldAttribute.DatabaseID);
                }
            }
            
            string formName = typeof(T).FullName;
            //if no group by and no statictisc
            //we should invoke GetEntryList
            if (!tr.HasGroupBy && !tr.HasStatictisc)
            {
                ARProxy<T> proxy = new ARProxy<T>((ARLoginContext)context);
                //get select
                List<uint> fieldIds = new List<uint>();
                if(tr.HasSelect && tr.SelectedProperties.Count > 0){
                    foreach(var s in tr.SelectedProperties){
                        fieldIds.Add(_Cache[formName][s.SourceMemberName]);
                    }
                }
                else{
                    foreach(var d in _Cache[formName])
                        fieldIds.Add(d.Value);
                }

                //get paged
                uint StartIndex = 0;
                uint? RetrieveCount = null;
                if(tr.HasSkip)
                    StartIndex = (uint)tr.Skip.Value;
                if(tr.HasTake)
                    RetrieveCount = (uint)tr.Take.Value;
                //do not return count
                //int total = -1;

                //get order by
                List<ARSortInfo> sort = new List<ARSortInfo>();
                if(tr.HasOrderBy){
                    foreach(var s in tr.OrderByList){
                        sort.Add(new ARSortInfo{
                            FieldId = _Cache[formName][s.Property],
                            Order = s.Method == "OrderBy" ? SortOrder.SORT_ASCENDING : SortOrder.SORT_DESCENDING
                        });
                    }
                }

                IList<T> resultList = proxy.GetEntryList(
                    tr.Qulification.ToString(),
                    fieldIds,
                    StartIndex,
                    RetrieveCount,
                    null,
                    sort
                    );

                if (tr.TargetType == null || !tr.HasSelect) //means no explict select clause
                {
                    return resultList;
                }
                else
                {
                    Type[] typeArgs = { tr.TargetType, typeof(T) };
                    return (IEnumerable)Activator.CreateInstance(
                            typeof(Enumerator<,>).MakeGenericType(typeArgs),
                            resultList,
                            tr.SelectExpression);
                }
            }
            else if (tr.HasStatictisc && tr.StatictiscVerb == "Count")
            {
                ARProxy<T> proxy = new ARProxy<T>((ARLoginContext)context);
                List<UInt32> groups = null;
                var list = proxy.GetListEntryStatictisc(
                    tr.Qulification.ToString(),
                    ARStatictisc.STAT_OP_COUNT,
                    null,
                    groups);
                if (list.Count != 1)
                    throw new Exception("GetListEntryStatictisc returns invalid result.");
                T entry = list[0];
                return Convert.ToInt32((entry as ARBaseForm).Statictisc);
 
            }
            else
            {
                throw new NotImplementedException("Group by and statictisc is not support yet.Please use ARProxy API");
                /*
                ARStatictisc stat = ARStatictisc.STAT_OP_COUNT;
                uint? targetFieldId = null;
                List<uint> groupByList = null;

                if(tr.HasStatictisc)
                {
                    switch (tr.StatictiscVerb)
                    {
                        case "Sum":
                            stat = ARStatictisc.STAT_OP_SUM;
                            break;
                        case "Max":
                            stat = ARStatictisc.STAT_OP_MAXIMUM;
                            break;
                        case "Min":
                            stat = ARStatictisc.STAT_OP_MINIMUM;
                            break;
                        case "Count":
                            stat = ARStatictisc.STAT_OP_COUNT;
                            break;
                        case "Average":
                            stat = ARStatictisc.STAT_OP_AVERAGE;
                            break;
                        default:
                            throw new NotImplementedException(tr.StatictiscVerb + " is not support.");
                    }

                    if(tr.StatictiscTarget == null && tr.StatictiscVerb != "Count")
                       throw new InvalidOperationException("TargetFieldId must not null if ARStat is not COUNT");

                    targetFieldId = _Cache[formName][tr.StatictiscTarget.SourceMemberName];
                }

                if(tr.HasGroupBy)
                {
                    groupByList = new List<uint>();
                    foreach(var g in tr.GroupedProperties){
                        groupByList.Add(_Cache[formName][g.SourceMemberName]);
                    }
                }



                ARProxy<T> proxy = new ARProxy<T>((ARLoginContext)context, (IARServerFactory)factory);
                IList<T> resultList = proxy.GetListEntryStatictisc(
                    tr.Qulification.ToString(),
                    stat,
                    targetFieldId,
                    groupByList);

                if (groupByList == null || groupByList.Count == 0)
                    return (resultList[0] as ARBaseForm).Statictisc;
                else
                {
                    Type[] typeArgs = { tr.GroupType, typeof(T) ,tr.TargetType };
                    return (IEnumerable)Activator.CreateInstance(
                            typeof(Groupor<,>).MakeGenericType(typeArgs),
                            resultList,
                            tr.GroupExpression);

 
                }
 */

            }

            #region code dropped

            ////two mainly type of query
            ////one with statictisc another without statictisc
            ////the flag is tr.HasStatictisc
            //if (tr.HasStatictisc)
            //{
            //    ARStatictisc stat;
            //    switch (tr.StatictiscVerb)
            //    {
            //        case "Sum":
            //            stat = ARStatictisc.STAT_OP_SUM;
            //            break;
            //        case "Max":
            //            stat = ARStatictisc.STAT_OP_MAXIMUM;
            //            break;
            //        case "Min":
            //            stat = ARStatictisc.STAT_OP_MINIMUM;
            //            break;
            //        case "Count":
            //            stat = ARStatictisc.STAT_OP_COUNT;
            //            break;
            //        case "Average":
            //            stat = ARStatictisc.STAT_OP_AVERAGE;
            //            break;
            //        default:
            //            throw new NotImplementedException(tr.StatictiscVerb + " is not support.");
                        
            //    }
            //    if (tr.HasGroupBy)
            //    {

            //    }
            //    else
            //    {

            //    }
            //    ARProxy<T> proxy = new ARProxy<T>((ARLoginContext)context);
            //    IList<T> list = proxy.GetListEntryStatictisc(
            //        tr.Qulification.ToString(),
            //        stat,
            //        Convert.ToUInt32(tr.StatictiscTarget.SourceMemberName),
            //        null);
            //    //two sub type of query
            //    //one with group by another without group by
            //    //the flag is tr.HasGroupBy

            //}
            //else
            //{
 
            //}

            //if (tr.HasGroupBy)
            //{
            //    ARProxy<T> proxy = new ARProxy<T>((ARLoginContext)context);
            //    IList<T> list = proxy.GetListEntryStatictisc(
            //        tr.Qulification.ToString(),
            //        ARStatictisc.STAT_OP_SUM,
            //        Convert.ToUInt32(tr.StatictiscTarget.SourceMemberName),
            //        null);
                
            //}

            //if (tr.Count)
            //{
            //    ARProxy4Linq<T> proxy = (ARProxy4Linq<T>)Activator.CreateInstance(
            //       typeof(ARProxy4Linq<T>), context, factory);
            //    return proxy.GetEntryCountByQuery(tr.Qulification.ToString());
            //}

            ////special "Select" clause apply
            //else if (tr.Select)
            //{
            //    tr.TargetType = TypeSystem.GetElementType(expression.Type);
            //    ARProxy4Linq<T> proxy = (ARProxy4Linq<T>)Activator.CreateInstance
            //        (typeof(ARProxy4Linq<T>), context, factory);

            //    return proxy.GetEntryByQuery(
            //        tr.Qulification.ToString(),
            //        tr.SelectedProperties,
            //        tr.TargetType,
            //        tr.SelectExpression
            //        );
            //}
            //else
            //{
            //    ARProxy4Linq<T> proxy = (ARProxy4Linq<T>)Activator.CreateInstance(
            //        typeof(ARProxy4Linq<T>), context, factory);
            //    return proxy.GetEntryByQuery(tr.Qulification.ToString());
            //}

            #endregion
        }

        public override string GetQueryText(Expression expression)
        {
            return this.Translate(expression).ToString();
        }

        private TranslateResult Translate(Expression expression)
        {
            return new QueryVisitor().Translate(expression);
        }

        private ARFieldAttribute GetARAttributeField(PropertyInfo pi, ModelBinderAccessLevel accessLevel)
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
    }


    public class SubtreeEvaluator : System.Linq.Expressions.ExpressionVisitor
    {

        public Expression Eval(Expression exp)
        {
            return this.Visit(exp);
        }

        public override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            if (exp.NodeType != ExpressionType.Parameter)
            {
                return this.Evaluate(exp);
            }
            return base.Visit(exp);
        }

        private Expression Evaluate(Expression e)
        {
            if (e.NodeType == ExpressionType.Constant)
            {
                return e;
            }
            LambdaExpression lambda = Expression.Lambda(e);
            Delegate fn = lambda.Compile();
            return Expression.Constant(fn.DynamicInvoke(null), e.Type);
        }

    }
}
