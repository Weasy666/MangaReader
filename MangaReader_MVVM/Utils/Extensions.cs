using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Utils
{
    static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            List<T> sorted = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }

        public static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;

            int i = 0;
            while (i < list.Count && comparer.Compare(list[i], item) < 0)
                i++;

            list.Insert(i, item);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        public static void SortAscending<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var comparer = new AscendingComparer<T>(comparison);

            List<T> sorted = collection.OrderBy(x => x, comparer).ToList();

            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }

        public static void SortDescending<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var comparer = new DescendingComparer<T>(comparison);

            List<T> sorted = collection.OrderBy(x => x, comparer).ToList();

            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }

    internal class AscendingComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> comparison;

        public AscendingComparer(Comparison<T> comparison)
        {
            this.comparison = comparison;
        }
        #region IComparer<T> Members  

        public int Compare(T x, T y)
        {
            return comparison.Invoke(x, y);
        }
        #endregion  
    }

    internal class DescendingComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> comparison;

        public DescendingComparer(Comparison<T> comparison)
        {
            this.comparison = comparison;
        }
        #region IComparer<T> Members  

        public int Compare(T x, T y)
        {
            return -comparison.Invoke(x, y);
        }
        #endregion  
    }
}