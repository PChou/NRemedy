using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace NRemedy.Linq
{
    public class QueryVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        //the return translateResult
        TranslateResult tr;

        public QueryVisitor(){
            tr = new TranslateResult();
        }

        internal TranslateResult Translate(Expression expression)
        {
            this.Visit(expression);
            return tr; 
        }
 
        private static Expression StripQuotes(Expression e) {
            while (e.NodeType == ExpressionType.Quote) {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }
 
        protected override Expression VisitMethodCall(MethodCallExpression m) {

            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where") {

                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                ConditionExpressionVisitor whereVisitor = new ConditionExpressionVisitor();
                whereVisitor.Translate(lambda.Body, tr);
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                SelectExpressionVisitor selectVisitor = new SelectExpressionVisitor();
                selectVisitor.Translate(lambda,tr);
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Count")
            {
                tr.HasStatictisc = true;
                tr.StatictiscVerb = "Count";
                this.Visit(m.Arguments[0]);
                if (m.Arguments.Count == 2)
                {
                    throw new NotSupportedException("Count clause not support parameter,use where clause instead.");
                }
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && (m.Method.Name == "OrderByDescending" || m.Method.Name == "OrderBy"))
            {
                Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                OrderByExpressionVisitor orderVisitor = new OrderByExpressionVisitor(m.Method.Name);
                orderVisitor.Translate(lambda.Body,tr);
                return m;
            }
            else if (m.Method.Name == "Sum" 
                || m.Method.Name == "Average" 
                || m.Method.Name == "Max" 
                || m.Method.Name == "Min" 
                || m.Method.Name == "Count")
            {
                throw new NotSupportedException("Statictisc is not support yet.");
            }

            else if (m.Method.Name == "GroupBy")
            {
                throw new NotSupportedException("GroupBy is not support yet.");
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }
 
        private Expression VisitConstant(ConstantExpression c,MemberExpression me) {
            if (me.Type.IsEnum)
                return Visit(Expression.Constant(Enum.GetName(me.Type, c.Value)));
            else
                return Visit(c);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            if (q != null)
            {
                //may occur where "Sikp" or "Take" is called
                if (q.Expression.NodeType == ExpressionType.Call){

                    NoQueryableExpressionVisitor nonq = new NoQueryableExpressionVisitor();
                    nonq.Translate(q.Expression,tr);
                }
            }
            return c;
        }
    }
}
