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

        StringBuilder sb;
        //the return translateResult
        TranslateResult tr;
        //indicate method stack , in order to tell which method is in when visit(),may be:
        Stack<string> methods = new Stack<string>();

        public QueryVisitor(){
            
            tr = new TranslateResult();
            methods = new Stack<string>();
        }

        internal TranslateResult Translate(Expression expression)
        {
            this.sb = new StringBuilder();
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

                //methods.Push("Where");
                this.tr.HasWhere = true;
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                ConditionExpressionVisitor whereVisitor = new ConditionExpressionVisitor();
                tr.Qulification = whereVisitor.Translate(lambda.Body).Qulification;
                //this.Visit(lambda.Body);

                //methods.Pop();

                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                //methods.Push("Select");
                this.tr.HasSelect = true;

                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                tr.SelectExpression = lambda;
                //add
                tr.TargetType = lambda.ReturnType;
                //this.Visit(lambda.Body);

                SelectExpressionVisitor selectVisitor = new SelectExpressionVisitor();
                SelectResult sr = selectVisitor.Translate(lambda.Body);
                tr.SelectedProperties = sr.SelectedProperties;
                

                //methods.Pop();
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Count")
            {
                tr.HasStatictisc = true;
                tr.StatictiscVerb = "Count";
                this.Visit(m.Arguments[0]);
                if (m.Arguments.Count == 2)
                {
                    //LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                    //this.Visit(lambda.Body);
                    throw new NotSupportedException("Count clause not support parameter,use where clause instead.");
                }
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && (m.Method.Name == "OrderByDescending" || m.Method.Name == "OrderBy"))
            {
                //methods.Push(m.Method.Name);

                tr.HasOrderBy = true;
                Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                OrderByExpressionVisitor orderVisitor = new OrderByExpressionVisitor(m.Method.Name);
                OrderByResult sr = orderVisitor.Translate(lambda.Body);
                tr.OrderByList = sr.OrderByList;

                //methods.Pop();
                return m;
            }
            else if (m.Method.Name == "Sum" 
                || m.Method.Name == "Average" 
                || m.Method.Name == "Max" 
                || m.Method.Name == "Min" 
                || m.Method.Name == "Count")
            {
                throw new NotSupportedException("Statictisc is not support yet.");
                /*
                if (tr.HasStatictisc)
                    throw new NotSupportedException("Multiple Statictisc Verb is not support.");
                
                methods.Push(m.Method.Name);
                tr.HasStatictisc = true;
                tr.StatictiscVerb = m.Method.Name;

                this.Visit(m.Arguments[0]);
                //Skip and Take must be ConstantExpression for Arguments[1]
                this.Visit(m.Arguments[1]);

                methods.Pop();

                return m;
                 */
            }

            else if (m.Method.Name == "GroupBy")
            {
                throw new NotSupportedException("GroupBy is not support yet.");

                //methods.Push(m.Method.Name);

                //tr.HasGroupBy = true;
                //this.Visit(m.Arguments[0]);
                //LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                //tr.GroupExpression = lambda;
                //tr.GroupType = lambda.ReturnType;
                //this.Visit(lambda.Body);

                //methods.Pop();
                //return m;
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
                    NoQueryableResult result =  nonq.Translate(q.Expression);
                    tr.HasSkip = result.HasSkip;
                    tr.HasTake = result.HasTake;
                    tr.Skip = result.Skip;
                    tr.Take = result.Take;
                }
            }
            return c;
        }

        //protected override Expression VisitMember(MemberExpression node)
        //{
        //    if (methods.Peek() == "OrderByDescending" || methods.Peek() == "OrderBy")
        //    {

        //        if (node.Member == null) return node;

        //        tr.OrderByList.Add(new SortInfo
        //        {
        //            Property = node.Member.Name,
        //            Method = methods.Peek()
        //        });
        //    }

        //    return node;
        //}
    }
}
