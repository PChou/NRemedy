using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NRemedy.Linq
{
    public class NoQueryableExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        private NoQueryableResult sr = null;

        private Stack<string> methods = new Stack<string>();

        public NoQueryableExpressionVisitor()
        {
            sr = new NoQueryableResult();
        }


        internal void Translate(Expression expression,TranslateResult translateResult)
        {
            if (expression == null || translateResult == null)
                return;
            this.Visit(expression);
            translateResult.NoQueryableResult = sr; ;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {

            if (m.Method.Name == "Skip")
            {
                methods.Push(m.Method.Name);

                sr.HasSkip = true;
                this.Visit(m.Arguments[0]);
                this.Visit(m.Arguments[1]);
                methods.Pop();

                return m;
            }
            else if (m.Method.Name == "Take")
            {
                methods.Push(m.Method.Name);
                sr.HasTake = true;
                this.Visit(m.Arguments[0]);
                this.Visit(m.Arguments[1]);
                methods.Pop();

                return m;
            }
          
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            if (q != null)
                return c;
            if (methods.Peek() == "Skip")
            {
                if (sr.Skip != null)
                    throw new NotSupportedException("Muti Skip expression is not supported.");
                sr.Skip = (int)c.Value;
            }
            else if (methods.Peek() == "Take")
            {
                if (sr.Take != null)
                    throw new NotSupportedException("Muti Take expression is not supported.");
                sr.Take = (int)c.Value;
            }
            return c;
        }
    }
}
