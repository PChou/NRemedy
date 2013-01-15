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

                methods.Push("Where");
                this.tr.HasWhere = true;
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);

                methods.Pop();

                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                methods.Push("Select");
                this.tr.HasSelect = true;

                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                tr.SelectExpression = lambda;
                //add
                tr.TargetType = lambda.ReturnType;
                this.Visit(lambda.Body);
                methods.Pop();
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
            else if (m.Method.DeclaringType == typeof(String) && m.Method.Name == "Contains")
            {
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
            else if (m.Method.DeclaringType == typeof(Queryable) && (m.Method.Name == "OrderByDescending" || m.Method.Name == "OrderBy"))
            {
                methods.Push(m.Method.Name);

                tr.HasOrderBy = true;
                Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);

                methods.Pop();
                return m;
            }
            else if (m.Method.Name == "Skip")
            {
                methods.Push(m.Method.Name);

                tr.HasSkip = true;
                
                this.Visit(m.Arguments[0]);
                //Skip and Take must be ConstantExpression for Arguments[1]
                this.Visit(m.Arguments[1]);

                methods.Pop();

                return m;
            }
            else if (m.Method.Name == "Take")
            {
                methods.Push(m.Method.Name);

                tr.HasTake = true;

                this.Visit(m.Arguments[0]);
                //Skip and Take must be ConstantExpression for Arguments[1]
                this.Visit(m.Arguments[1]);

                methods.Pop();

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
 
        protected override Expression VisitUnary(UnaryExpression u) {
            switch (u.NodeType) {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
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
                if (methods.Peek() == "Where")
                {
                    switch (Type.GetTypeCode(c.Value.GetType()))
                    {
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
                        default:
                            throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                    }
                }
                else if (methods.Peek() == "Skip")
                {
                    if(tr.Skip != null)
                        throw new NotSupportedException("Muti Skip expression is not supported.");
                    tr.Skip = (int)c.Value;
 
                }
                else if (methods.Peek() == "Take")
                {
                    if(tr.Take != null)
                        throw new NotSupportedException("Muti Take expression is not supported.");
                    tr.Take = (int)c.Value;
                }
            }
            return c;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (methods.Peek() == "Where")
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

                string arFieldName = attr.DatabaseName;
                if (string.IsNullOrEmpty(arFieldName)) throw new CustomAttributeFormatException(
                        string.Format("DatabaseName in ARFieldAttribute is found on Property : {0}.", node.Member.Name)
                    );

                tr.Qulification.Append("'");
                tr.Qulification.Append(arFieldName);
                tr.Qulification.Append("'");
            }
            else if (methods.Peek() == "Select")
            {
                tr.SelectedProperties.Add(new MemberMap { 
                    SourceMemberName = node.Member.Name
                });
            }
            //else if (methods.Peek() == "GroupBy")
            //{
            //    tr.GroupedProperties.Add(new MemberMap
            //    {
            //        SourceMemberName = node.Member.Name
            //    });
            //}
            //else if (methods.Peek() == "Sum"
            //    || methods.Peek() == "Average"
            //    || methods.Peek() == "Max"
            //    || methods.Peek() == "Min"
            //    || methods.Peek() == "Count") {

            //        tr.StatictiscTarget = new MemberMap {
            //            SourceMemberName = node.Member.Name
            //        };
            //}
            else if (methods.Peek() == "OrderByDescending" || methods.Peek() == "OrderBy")
            {

                if (node.Member == null) return node;

                //object[] attrs = node.Member.GetCustomAttributes(typeof(ARFieldAttribute), true);
                //if (attrs.Length > 1)
                //    throw new CustomAttributeFormatException(
                //        string.Format("Mutiple ARFieldAttribute is found on Property : {0}.", node.Member.Name));
                //if (attrs.Length == 0)
                //    throw new CustomAttributeFormatException(
                //        string.Format("No ARFieldAttribute is found on Property : {0}.", node.Member.Name));
                //ARFieldAttribute attr = (ARFieldAttribute)attrs.First();
                ////attr.
                //if (tr.OrderByList == null)
                //    tr.OrderByList = new List<SortInfo>();
                tr.OrderByList.Add(new SortInfo
                {
                    Property = node.Member.Name,
                    Method = methods.Peek()
                });
            }

            return node;
        }

        //protected override Expression VisitMemberInit(MemberInitExpression node)
        //{
        //    //only select clause may visit member init
        //    if(methods.Peek() == "Select")
        //    {
        //        tr.TargetType = node.Type;
                
        //        foreach (var bind in node.Bindings)
        //        {
        //            if (bind.BindingType == MemberBindingType.Assignment)
        //            {
        //                MemberAssignment ma = bind as MemberAssignment;
        //                MemberMap memberMap = new MemberMap();
        //                if(ma == null)
        //                    throw new NullReferenceException("bind");
        //                memberMap.TargetMemberName = ma.Member.Name;
        //                if(!(ma.Expression is MemberExpression))
        //                    throw new NotImplementedException("Non-MemberExpression is not support yet.");
        //                memberMap.SourceMemberName = (ma.Expression as MemberExpression).Member.Name;
        //                tr.SelectedProperties.Add(memberMap);
        //            }
        //            else if(bind.BindingType == MemberBindingType.ListBinding)
        //            {
        //                throw new NotImplementedException("MemberBindingType.ListBinding is not support yet.");
        //            }
        //            else if (bind.BindingType == MemberBindingType.MemberBinding)
        //            {
        //                throw new NotImplementedException("MemberBindingType.MemberBinding is not support yet.");
        //            }
        //        }
        //    }
            

        //    return node;

        //}

        //protected override Expression VisitNew(NewExpression node)
        //{
        //    //only select clause may visit member init
        //    if (methods.Peek() == "Select")
        //    {
        //        tr.TargetType = node.Type;
        //        for (int i = 0; i < node.Members.Count; i++)
        //        {
        //            MemberMap memberMap = new MemberMap();
        //            //memberMap.TargetMemberName = node.Members[i].Name;
        //            //if(!(node.Arguments[i].Type is MemberExpression))
        //            //        throw new NotImplementedException("Non-MemberExpression is not support yet.");
        //            //    memberMap.SourceMemberName = (ma.Expression as MemberExpression).Member.Name;
                    
        //        }
        //    }
        //    return node;
        //}
    }
}
