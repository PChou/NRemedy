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
            tr.SelectedProperties = new List<MemberMap>();
        }

        internal SelectResult Translate(Expression expression)
        {
            this.Visit(expression);
            return tr;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            tr.SelectedProperties.Add(new MemberMap
            {
                SourceMemberName = node.Member.Name
            });

            return node;
        }
    }
}
