using System.Linq;

namespace System.Collections.Generic
{
    public class BList<T> : IList<T>, IReadOnlyList<T>
    {
        private const int InitialCapacity = 4;
        private const int InitialOffset = 2;

        private int _size;
        private int _offset;
        private int _capacity;

        private T[] _items;

        public BList()
        {
            _size = 0;
            _offset = InitialOffset;
            _capacity = InitialCapacity;
            _items = new T[InitialCapacity];
        }



        private void Insert(int insertIndex, T[] insertItems)
        {
            if (insertIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(insertIndex));

            var insertCount = insertItems.Length;

            var padRight = Math.Max(0, (insertIndex == 0)
                ? (insertCount - (_size + 1))
                : (insertIndex + insertCount) - (_size));

            var padLeft = insertIndex == 0
                ? insertCount
                : 0;

            var requiresResize = _offset - padLeft < 0 ||
                                 _offset + _size + padRight > _capacity;

            if (requiresResize)
            {
                var newSize = _size + insertCount;
                var newCapacity = Math.Max(newSize, _capacity*2);
                var newOffset = (newCapacity/2) - (newSize/2) - padLeft;
                var newItems = new T[newCapacity];


                for (int i = 0; i < insertIndex; i++)
                    newItems[newOffset + i] = _items[_offset + i];
        
                for (int i = insertIndex; i < _size; i++)
                    newItems[newOffset + i + insertCount] = _items[_offset + i];

                for (int i = 0; i < insertCount; i++)
                    newItems[newOffset + insertIndex + i] = insertItems[i];
        
                _items = newItems;
                _offset = newOffset;
                _size = newSize;
                _capacity = newCapacity;
            }
            else
            {

                if (insertIndex != 0)
                {
                    //for (int i = 0; i < insertIndex; i++)
                    //    _items[_offset + i] = _items[_offset + i];

                    for (int i = _size - 1; i >= insertIndex; i--)
                        _items[_offset + i + insertCount] = _items[_offset + i];

                    for (int i = 0; i < insertCount; i++)
                        _items[_offset + insertIndex + i] = insertItems[i];
                }
                else
                {
                    _offset = _offset - insertCount;

                    for (int i = 0; i < insertCount; i++)
                        _items[_offset + insertIndex + i] = insertItems[i];
                }

                _size = _size + insertCount;
            }

        }

        public int Count => _size;

        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            for (int i = 0; i < _size ; i++)
            {
                if (_items[i+_offset].Equals(item))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);

            if (index == -1)
                return false;

            RemoveAt(index);

            return true;
        }

        public void RemoveAt(int index)
        {
            if (index == 0)
            {
                _items[_offset] = default(T);
                _offset++;
                _size -= 1;
            }
            else if (index == _size - 1)
            {
                _items[_offset + _size - 1] = default(T);
                _size -= 1;
            }
            else
            {
                for (int i = _offset + index; i < _size - 1; i++)
                    _items[i] = _items[i + 1];

                _size -= 1;
            }
        }

        public T this[int index]
        {
            get { return _items[_offset + index]; }
            set { Insert(index, new[] { value }); }
        }

        public void Insert(int insertIndex, T item)
        {
            Insert(insertIndex, new[] { item });
        }
        
        public void Add(T item)
        {
            Insert(Count, new[] { item });
        }

        public void AddRange(IEnumerable<T> collection)
        {
            Insert(_size, collection.ToArray());
        }

        public void Clear()
        {
            _offset = _capacity / 2;
            _size = 0;
        }

        public bool Contains(T item)
        {
            if ((Object)item == null)
            {
                for (int i = 0; i < _size; i++)
                    if ((Object)_items[i] == null)
                        return true;
                return false;
            }
            else
            {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < _size; i++)
                {
                    if (c.Equals(_items[i], item)) return true;
                }
                return false;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _offset; i < _offset + _size; i++)
                yield return _items[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    };
}
