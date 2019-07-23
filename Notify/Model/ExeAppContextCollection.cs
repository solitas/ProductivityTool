using System;
using System.Collections;
using System.Collections.Generic;

namespace ProductivityTool.Notify.Model
{
    public class ExeAppContextCollection : ICollection<ExeAppContext>
    {
        private List<ExeAppContext> _items = new List<ExeAppContext>();

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public ExeAppContextCollection()
        {

        }

        public void Add(ExeAppContext item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(ExeAppContext item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(ExeAppContext[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ExeAppContext> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public bool Remove(ExeAppContext item)
        {
            return _items.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void Remove(Predicate<ExeAppContext> p)
        {
            var invalidContext = _items.RemoveAll(p);
        }
    }
}
