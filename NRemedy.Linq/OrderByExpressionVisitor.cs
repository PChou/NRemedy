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

        internal void Translate(Expression expression, TranslateResult translateResult)
        {
            if (expression == null || translateResult == null)
                return;
            this.Visit(expression);
            if (translateResult.OrderByResult == null)
                translateResult.OrderByResult = sr;
            else
            {
                foreach (var i in sr.OrderByList)
                {
                    translateResult.OrderByResult.OrderByList.Add(i);
                }
            }
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
