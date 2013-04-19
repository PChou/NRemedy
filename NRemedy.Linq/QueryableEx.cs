using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRemedy.Linq
{
    public static class QueryableEx
    {
        /// <summary>
        /// translate the linq expression only
        /// </summary>
        /// <typeparam name="S">source Type in IQueryable , usually is the type of the ARSet</typeparam>
        /// <param name="IQ">IQueryble instance</param>
        /// <returns>TranslateResult</returns>
        public static TranslateResult Translate<S>(this IQueryable IQ)
            where S : ARBaseForm
        {
            if (!(IQ.Provider is ARQueryProvider<S>))
                throw new NotSupportedException("The Provider of target IQueryable is not ARQueryProvider");
            return ((ARQueryProvider<S>)IQ.Provider).ExecuteOnlyTranslate(IQ.Expression);
        }

    }
}
