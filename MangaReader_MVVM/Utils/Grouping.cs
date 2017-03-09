using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MangaReader_MVVM.Utils
{
    public class Grouping<TKey, TElement> : ObservableCollection<TElement>, IGrouping<TKey, TElement>
    {
        public Grouping(TKey key)
        {
            this.Key = key;
        }

        public Grouping(TKey key, IEnumerable<TElement> items)
            : this(key)
        {
            foreach (var item in items)
            {
                this.AddSorted(item);
            }
        }

        public TKey Key { get; }
    }

    //public class Grouping<K, T> : ObservableCollection<T>
    //{
    //    public K Key { get; private set; }

    //    public Grouping(K key, IEnumerable<T> items)
    //    {
    //        Key = key;
    //        foreach (var item in items)
    //        {
    //            this.Items.Add(item);
    //        }
    //    }
    //}
}
