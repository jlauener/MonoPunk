using System;
using System.Collections.Generic;

namespace MonoPunk
{
    internal class SmartList<T>
    {
        internal delegate void Callback(T elem);

        private readonly List<T> _list = new List<T>();
        private readonly List<T> _addList = new List<T>();
        private readonly List<T> _removeList = new List<T>();

        internal void Add(T elem)
        {
            _addList.Add(elem);
        }

        internal void Remove(T elem)
        {
            _removeList.Add(elem);
        }

        internal void FlushAddList(Callback callback = null)
        {
            if(_addList.Count > 0)
            {
                foreach(var elem in _addList)
                {
                    _list.Add(elem);
                    if(callback != null)
                    {
                        callback(elem);
                    }
                }
                _addList.Clear();
            }
        }

        internal void FlushRemoveList(Callback callback = null)
        {
            if(_removeList.Count > 0)
            {
                foreach(var elem in _removeList)
                {
                    if(callback != null)
                    {
                        callback(elem);
                    }                    
                    _list.Remove(elem);
                }
                _removeList.Clear();
            }
        }

        internal List<T> GetList()
        {
            return _list;
        }
    }
}
