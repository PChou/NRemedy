using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Text;

namespace NRemedy.Linq
{
    /// <summary>
    /// linq to remedy的IQueryable实现
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    public class ARSet<T> : Query<T>
        where T : ARBaseForm
    {
        public ARSet(object context)
            : base(new ARQueryProvider<T>(context))
        {
        }

    }
}
