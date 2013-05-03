using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NRemedy.Linq
{
    public class ConditionExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        //the return translateResult
        ConditionResult tr;

        public ConditionExpressionVisitor()
        {
            tr = new ConditionResult();
            tr.Qulification = new StringBuilder();
        }

        internal ConditionResult Translate(Expression expression)
        {
            if (expression == null)
                return null;
            this.Visit(expression);
            return tr;
        }

        internal void Translate(Expression expression,TranslateResult translateResult)
        {
            if (expression == null || translateResult == null)
                return;
            this.Visit(expression);

            translateResult.ConditionResult = tr;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m) {

            if (m.Method.DeclaringType == typeof(String) && m.Method.Name == "Contains"){
                tr.Qulification.Append("(");
                Visit(m.Object);
                tr.Qulification.Append(" LIKE ");
                if (m.Arguments[0].NodeType == ExpressionType.Constant && m.Object.NodeType == ExpressionType.MemberAccess)
                {
                    VisitConstant((ConstantExpression)m.Arguments[0], (MemberExpression)m.Object);
                }
                tr.Qulification.Append(")");
                return m;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported in ConditionExpressionVisitor", m.Method.Name));
        }
 
        protected override Expression VisitUnary(UnaryExpression u) {
            switch (u.NodeType) {
                case ExpressionType.Not:
                    tr.Qulification.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }
 
        protected override Expression VisitBinary(BinaryExpression b) {
            tr.Qulification.Append("(");
            Expression eTmp = this.Visit(b.Left);
            switch (b.NodeType) {
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    tr.Qulification.Append(" AND ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    tr.Qulification.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    tr.Qulification.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    tr.Qulification.Append(" != ");
                    break;
                case ExpressionType.LessThan:
                    tr.Qulification.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    tr.Qulification.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    tr.Qulification.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    tr.Qulification.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            if (b.Right.NodeType == ExpressionType.Constant && eTmp.NodeType == ExpressionType.MemberAccess){
                VisitConstant((ConstantExpression)b.Right, (MemberExpression)eTmp);
            }
            else
                this.Visit(b.Right);
            tr.Qulification.Append(")");
            return b;
        }
 
        private Expression VisitConstant(ConstantExpression c,MemberExpression me) {
            if (me.Type.IsEnum)
                return Visit(Expression.Constant(Enum.GetName(me.Type, c.Value)));
            else
                return Visit(c);
        }

        protected override Expression VisitConstant(ConstantExpression c) {
            IQueryable q = c.Value as IQueryable;
            if (q != null) {
                //may occur where "Sikp" or "Take" is called
                if (q.Expression.NodeType == ExpressionType.Call)
                {
                    this.Visit(q.Expression);                   
                }
            }
            else if (c.Value == null) {
                tr.Qulification.Append("$NULL$");
            }
            else {
                TypeCode tc = Type.GetTypeCode(c.Value.GetType());
                switch (tc){
                    case TypeCode.String:
                        tr.Qulification.Append("\"");
                        tr.Qulification.Append(c.Value);
                        tr.Qulification.Append("\"");
                        break;
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                        tr.Qulification.Append(c.Value);
                        break;
                    case TypeCode.DateTime://value should support datetime
                        tr.Qulification.Append("\"");
                        tr.Qulification.Append(c.Value);
                        tr.Qulification.Append("\"");
                        break;
                    case TypeCode.Boolean:
                    //!!!from now on when typecode is boolean, this means the ConstantExpression must be
                    //a single expression and not a right expression of BinaryExpression.
                    //This is because AR not support true/false field type,and the model should not contains bool type property
                    //If someone forcely using bool property as where filter,the qulification building will be incorrect
                        if(Convert.ToBoolean(c.Value) == true)
                            tr.Qulification.Append(" 1 = 1 ");
                        else
                            tr.Qulification.Append(" 1 != 1 ");
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                }
            }
            return c;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member == null) return node;

            object[] attrs = node.Member.GetCustomAttributes(typeof(ARFieldAttribute), true);
            if (attrs.Length > 1)
                throw new CustomAttributeFormatException(
                    string.Format("Mutiple ARFieldAttribute is found on Property : {0}.", node.Member.Name));
            if (attrs.Length == 0)
                throw new CustomAttributeFormatException(
                    string.Format("No ARFieldAttribute is found on Property : {0}.", node.Member.Name));
            ARFieldAttribute attr = (ARFieldAttribute)attrs.First();

            uint arFieldId = attr.DatabaseID;
            if (arFieldId == default(uint)) throw new CustomAttributeFormatException(
                    string.Format("Invalid DatabaseId in ARFieldAttribute is found on Property : {0}.", node.Member.Name)
                );

            tr.Qulification.Append("'");
            tr.Qulification.Append(arFieldId);
            tr.Qulification.Append("'");

            return node;
        }
    }
}
