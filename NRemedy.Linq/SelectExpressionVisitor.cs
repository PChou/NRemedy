using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NRemedy.Linq
{
    public class SelectExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        //the return translateResult
        SelectResult tr;

        public SelectExpressionVisitor()
        {
            tr = new SelectResult();
            tr.SelectedProperties = new List<string>();
        }

        internal void Translate(LambdaExpression expression,TranslateResult translateResult)
        {
            if (expression == null || translateResult == null)
                return;
            this.Visit(expression.Body);//get selected properties

            tr.TargetType = expression.ReturnType;
            tr.SelectExpression = expression;
            translateResult.SelectResult = tr;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            tr.SelectedProperties.Add(node.Member.Name);
            return node;
        }
    }
}
