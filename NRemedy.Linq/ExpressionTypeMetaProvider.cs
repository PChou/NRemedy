using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace NRemedy.Linq
{
    public class ExpressionTypeMetaProvider<TModel> : DefaultTypeMetaProvider<TModel>//, IExpressionMetaProvider<TModel>
    {
        private Dictionary<PropertyInfo, GetAndSetExpression> _properties = new Dictionary<PropertyInfo, GetAndSetExpression>();

        public ExpressionTypeMetaProvider(Expression<Func<TModel, object>>[] props)
        {
            foreach (Expression<Func<TModel, object>> expression in props)
            {
                MemberExpression memeberExp = null;

                //for value type , the expression will become Convert Type,since value type
                //need to box to reference type
                if (expression.Body.NodeType == ExpressionType.Convert)
                {
                    Expression inner = ((UnaryExpression)expression.Body).Operand;
                    if (inner.NodeType != ExpressionType.MemberAccess)
                        throw new ArgumentException("Argument Expression must be type of ExpressionType.MemberAccess");
                    memeberExp = inner as MemberExpression;
                }
                else
                {
                    if (expression.Body.NodeType != ExpressionType.MemberAccess)
                        throw new ArgumentException("Argument Expression must be type of ExpressionType.MemberAccess");
                    memeberExp = expression.Body as MemberExpression;
                }
                if (memeberExp.Member.MemberType != MemberTypes.Property)
                    throw new ArgumentException("Argument Expression's MemberType must be type of MemberTypes.Property");

                //build setter expression
                //var m = Expression.Parameter(typeof(TModel),"m");
                //var v = Expression.Parameter(typeof(object),"v");
                //var ma = Expression.Bind(memeberExp.Member,v);
                //var actionExp = Expression.Lambda<Action<TModel, object>>(ma.Expression, m, v);
                
                _properties.Add(memeberExp.Member as PropertyInfo, 
                    
                    new GetAndSetExpression(){
                    
                           Getter = expression.Compile(),
                           //Setter = actionExp.Compile()
                    
                        }
                    );
            }
        }

        public override IEnumerable<PropertyAndField<TModel>> GetPropertyInfoes
            (System.Reflection.BindingFlags bindFlags, PropertyFilterDelegate2 filter)
        {
            Type ModelType = typeof(TModel);
            IEnumerable<PropertyInfo> properties = _properties.Select(e => e.Key);
            List<PropertyAndField<TModel>> list = new List<PropertyAndField<TModel>>();
            foreach (var p in properties)
            {
                if (filter != null)
                {
                    if (!filter(p)) continue;
                }
                PropertyAndField<TModel> pf = new PropertyAndField<TModel>();
                pf.DatabaseId = GetDatabaseIdFromPropertyInfo(p);
                pf.DatabaseName = GetDatabaseNameFromPropertyInfo(p);
                pf.DatabaseType = GetDataTypeFromPropertyInfo(p);
                pf.AccessLevel = GetAccessLevelFromPropertyInfo(p);
                pf.Property = p;
                pf.Getter = _properties[p].Getter;
                pf.Setter = _properties[p].Setter;
                list.Add(pf);
            }

            return list;
        }



        private sealed class GetAndSetExpression
        {
            public Func<TModel, object> Getter { get; set; }

            public Action<TModel, object> Setter { get; set; }
        }




    }
}
