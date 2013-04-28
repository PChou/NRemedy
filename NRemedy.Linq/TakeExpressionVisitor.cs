using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NRemedy.Linq
{
    public class OrderByExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        private OrderByResult sr;
        private string direction;

        public OrderByExpressionVisitor(string OrderDirection)
        {
            sr = new OrderByResult();
            sr.OrderByList = new List<SortInfo>();
            direction = OrderDirection;
        }

        internal OrderByResult Translate(Expression expression)
        {
            this.Visit(expression);
            return sr;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member == null) return node;

            sr.OrderByList.Add(new SortInfo
            {
                Property = node.Member.Name,
                Method = direction
            });

            return node;
        }
    }
}
