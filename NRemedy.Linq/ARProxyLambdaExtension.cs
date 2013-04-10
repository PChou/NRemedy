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

namespace NRemedy.Linq
{
    /// <summary>
    /// ARProxy extension method,argument for Lambda expression
    /// </summary>
    public static class ARProxyLambdaExtension
    {
        public static void DeleteEntries<TModel>(this ARProxy<TModel> proxy, Expression<Func<TModel, bool>> expression)
        {
            //TODO : using cached Reflect method to get entryid from model
            DeleteEntries(proxy, expression, null);
        }
        
        internal static void DeleteEntries<TModel>(this ARProxy<TModel> proxy, Expression<Func<TModel,bool>> expression, Func<TModel,string> fnGetEntryId)
        {
            if (expression == null)
                throw new ArgumentNullException("expression must not null. If want to delete all entries, try to use m => true");

            ConditionExpressionVisitor visitor = new ConditionExpressionVisitor();
            var expEvaled = Evaluator.PartialEval(expression);
            ConditionResult tr = visitor.Translate(expEvaled);
            string qu = tr.Qulification == null ? null : tr.Qulification.ToString();
            if (fnGetEntryId == null)
                proxy.DeleteEntriesByQuery(qu);
            else
                proxy.DeleteEntriesByQuery(qu,fnGetEntryId); 
        }
    }
}
