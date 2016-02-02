using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public class BList<T> : IList<T>, IReadOnlyList<T>
    {
        private const int InitialCapacity = 16;
        private const int InitialOffset = 8;

        private int _size;
        private int _offset;
        private int _capacity;
        private int _version;

        private T[] _items;

        public BList()
        {
            _version = 0;
            _size = 0;
            _offset = InitialOffset;
            _capacity = InitialCapacity;
            _items = new T[InitialCapacity];
        }

        public BList(int initialCapacity)
        {
            _size = 0;
            _version = 0;
            _offset = initialCapacity / 2;
            _capacity = initialCapacity;
            _items = new T[initialCapacity];
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

                Array.Copy(_items, _offset, newItems, newOffset, insertIndex);
                Array.Copy(_items, _offset, newItems, newOffset + 1, _size - insertIndex);
                Array.Copy(insertItems, 0, newItems, newOffset + insertIndex, insertCount);

                _items = newItems;
                _offset = newOffset;
                _size = newSize;
                _capacity = newCapacity;
            }
            else
            {
                if (insertIndex == 0)
                    _offset = _offset - 1;
                else if (insertIndex < _size)
                    Array.Copy(_items, _offset + insertIndex, _items, _offset + insertIndex + 1, _size - insertIndex);

                Array.Copy(insertItems, 0, _items, _offset + insertIndex, insertCount);

                _size = _size + insertCount;
            }

            _version++;
        }

        public void Insert(int insertIndex, T item)
        {
            if (insertIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(insertIndex));

            var padRight = Math.Max(0, (insertIndex == 0)
                ? -_size
                : (insertIndex + 1) - _size);

            var padLeft = insertIndex == 0
                ? 1
                : 0;

            var requiresResize = _offset - padLeft <= 0 ||
                                 _offset + _size + padRight >= _capacity;
            if (requiresResize)
            {
                var newSize = _size + 1;
                var newCapacity = Math.Max(newSize, _capacity * 2);
                var newOffset = (newCapacity / 2) - (newSize / 2) - padLeft;
                var newItems = new T[newCapacity];

                Array.Copy(_items, _offset, newItems, newOffset, insertIndex);
                Array.Copy(_items, _offset, newItems, newOffset + 1, _size - insertIndex);

                newItems[newOffset + insertIndex] = item;

                _items = newItems;
                _offset = newOffset;
                _size = newSize;
                _capacity = newCapacity;
            }
            else
            {
                if (insertIndex == 0)
                    _offset = _offset - 1;
                else if (insertIndex < _size)
                    Array.Copy(_items, _offset + insertIndex, _items, _offset + insertIndex + 1, _size - insertIndex);

                _items[_offset + insertIndex] = item;

                _size = _size + 1;
            }

            _version++;
        }

        public void Add(T item)
        {
            var padRight = 1;

            var requiresResize = _offset + _size + padRight >= _capacity;

            if (requiresResize)
            {
                var newSize = _size + 1;
                var newCapacity = Math.Max(newSize, _capacity * 2);
                var newOffset = (newCapacity/2) - (newSize/2);
                var newItems = new T[newCapacity];

                Array.Copy(_items, _offset, newItems, newOffset, _size);

                newItems[newOffset + _size] = item;

                _items = newItems;
                _offset = newOffset;
                _size = newSize;
                _capacity = newCapacity;
            }
            else
            {
                _items[_offset + _size] = item;

                _size += 1;
            }

            _version++;
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

            _version++;
        }

        public T this[int index]
        {
            get { return _items[_offset + index]; }
            set { _items[_offset + index] = value; _version++; }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            Insert(_size, collection.ToArray());
        }

        public void Clear()
        {
            _offset = _capacity / 2;
            _size = 0;
            _version++;
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
            Array.Copy(_items, _offset, array, arrayIndex, _size);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var versionStart = _version;

            for (int i = _offset; i < _offset + _size; i++)
            {
                if (_version != versionStart)
                    throw new InvalidOperationException(
                        "Collection was modified; enumeration operation may not execute.");

                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    };
}
