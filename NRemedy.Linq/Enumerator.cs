using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;

namespace NRemedy.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">target type</typeparam>
    /// <typeparam name="K">src type</typeparam>
    public class Enumerator<T, K> : IEnumerator<T>, IEnumerable<T>, IEnumerator, IEnumerable
    {
        private Func<K, T> _wrapper;
        private int _currentIndex = -1;
        private T _current;
        private IEnumerable<K> _srcModels;

        public Enumerator(IEnumerable<K> srcModels, LambdaExpression wrapper)
        {
            _wrapper = (Func<K, T>)wrapper.Compile();
            _srcModels = srcModels;
        }


        public T Current
        {
            get { return _current; }
        }

        public void Dispose()
        {
            return;
        }

        object IEnumerator.Current
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
                _current = _wrapper(_srcModels.ElementAt(_currentIndex));
                return true;
            }
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
