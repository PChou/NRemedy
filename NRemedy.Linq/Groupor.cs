using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NRemedy.Linq
{
    /// <summary>
    /// groupor for wrap group by and select result
    /// </summary>
    /// <typeparam name="TKey">key type usually indicate in group by clause</typeparam>
    /// <typeparam name="TElement">result type usually source type</typeparam>
    public class Groupor<TKey, TElement> : IGrouping<TKey, TElement>, IEnumerator, IEnumerator<TElement>
    {
        IEnumerable<TElement> _srcModels;
        Func<TElement, TKey> _groupwrapper;
        //Func<TKey, TTarget> _selectwrapper;
        private int _currentIndex = -1;
        private TElement _current;


        public Groupor(IEnumerable<TElement> srcModels, 
            LambdaExpression groupWrapper
            )
        {
            if (srcModels == null)
                throw new ArgumentNullException("srcModels");
            if (groupWrapper == null)
                throw new ArgumentNullException("groupWrapper");
            //if (selectWrapper == null)
            //    throw new ArgumentNullException("selectwrapper");

            _srcModels = srcModels;
            _groupwrapper = (Func<TElement, TKey>)groupWrapper.Compile();
            //_selectwrapper = (Func<TKey, TTarget>)selectWrapper.Compile();
        }

        public TKey Key
        {
            get {
                return _groupwrapper(_current);
            }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public object Current
        {
            get
            {
                return _current;
            }
        }

        public bool MoveNext()
        {
            _currentIndex++;
            if (_currentIndex >= _srcModels.Count())
                return false;
            else
            {
                _current = _srcModels.ElementAt(_currentIndex);
                return true;
            }
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        TElement IEnumerator<TElement>.Current
        {
            get
            {
                return _current;
            }
        }

        public void Dispose()
        {
            return;
        }
    }
}
