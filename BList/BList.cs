using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    public class BList<T> : IList<T>, IReadOnlyList<T>, INotifyCollectionChanged
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
            _offset = initialCapacity/2;
            _capacity = initialCapacity;
            _items = new T[initialCapacity];
        }

        public void Insert(int insertIndex, T item)
        {
            if (insertIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(insertIndex));

            var padRight = Math.Max(0, (insertIndex == 0) ? 0 : (insertIndex + 1) - _size);
            var padLeft = insertIndex == 0 ? 1 : 0;

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

            if (CollectionChanged != null)
                RaiseCollectionAdd(insertIndex, item);
        }


        public void Insert(int insertIndex, T[] insertItems)
        {
            if (insertIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(insertIndex));

            var insertCount = insertItems.Length;

            var padRight = Math.Max(0, (insertIndex == 0)
                ? _size - insertCount
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
                var newOffset = Math.Max(0, (newCapacity/2) - (newSize/2) - padLeft);
                var newItems = new T[newCapacity];

                Array.Copy(_items, _offset, newItems, newOffset, insertIndex);
                Array.Copy(_items, _offset, newItems, newOffset + insertCount, _size - insertIndex);
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

            if (CollectionChanged != null)
                RaiseCollectionAdd(insertIndex, insertItems);
        }

        

        public void Add(T item)
        {
            var padRight = 1;

            var requiresResize = _offset + _size + padRight >= _capacity;

            if (requiresResize)
            {
                var newSize = _size + 1;
                var newCapacity = Math.Max(newSize, _capacity*2);
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

            if(CollectionChanged!= null)
                RaiseCollectionAdd(_size - 1, item);
        }

        public int Count => _size;

        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            if ((Object) item == null)
            {
                for (int i = 0; i < _size; i++)
                    if ((Object) _items[_offset + i] == null)
                        return i;
                return -1;
            }
            else
            {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < _size; i++)
                {
                    if (c.Equals(_items[_offset + i], item)) return i;
                }
                return -1;
            }
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
            var item = _items[_offset + index];

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

            RaiseCollectionRemove(index, item);
        }

        public T this[int index]
        {
            get { return _items[_offset + index]; }
            set
            {
                var old = _items[_offset + index];
                _items[_offset + index] = value;
                _version++;
                RaiseCollectionReplace(index, value, old);
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            Insert(_size, collection.ToArray());
        }

        public void ReplaceRange(IEnumerable<T> collection, int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            var newItems = new List<T>();
            var oldItems = new List<T>();

            foreach (var item in collection)
            {
                if (index >= _size)
                    break;

                oldItems.Add(_items[_offset + index]);
                _items[_offset + index] = item;
                newItems.Add(item);
                index += 1;
            }

            if (newItems.Count > 0)
                RaiseCollectionReplace(index, newItems, oldItems);
        }

        public void Clear()
        {
            _offset = _capacity/2;
            _size = 0;
            _version++;

            RaiseCollectionReset();
        }

        public bool Contains(T item)
        {
            if ((Object) item == null)
            {
                for (int i = 0; i < _size; i++)
                    if ((Object) _items[i] == null)
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

        protected virtual void RaiseCollectionReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        protected virtual void RaiseCollectionRemove(int index, IList oldItems)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, oldItems, index));
        }

        protected virtual void RaiseCollectionRemove(int index, T oldItem)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        protected virtual void RaiseCollectionReplace(int index, IList newItems, IList oldItems)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace, newItems, oldItems, index));
        }

        protected virtual void RaiseCollectionReplace(int index, T newItem, T oldItem)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
        }

        protected virtual void RaiseCollectionAdd(int index, IList newItems)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, newItems, index));
        }

        protected virtual void RaiseCollectionAdd(int index, T newItem)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, newItem, index));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

    };
}
