using System;
using System.Collections.ObjectModel;

namespace MangaReader_MVVM.Models
{
    
    public interface IChapter : IComparable<IChapter>, IEquatable<IChapter>
    {
        IManga ParentManga { get; set; }
        float Number { get; set; }
        string Title { get; set; }
        string Id { get; set; }
        DateTime Released { get; set; }
        bool IsRead { get; set; }
        ObservableCollection<IPage> Pages { get; set; }
    }
}
