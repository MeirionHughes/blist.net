using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Collections.Specialized
{
    public interface IReadOnlyNotifyList<out T> :
        IReadOnlyList<T>,
        INotifyCollectionChanged
    {
    }
}