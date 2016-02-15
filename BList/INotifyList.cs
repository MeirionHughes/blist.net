using System.Collections.Specialized;

namespace System.Collections.Generic
{
    public interface INotifyList<T> :
        IList<T>, INotifyCollectionChanged
    {
    }
}