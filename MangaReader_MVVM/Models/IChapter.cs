using MangaReader_MVVM.Converters.JSON;
using Newtonsoft.Json;
using System;
using Template10.Controls;
using Template10.Mvvm;

namespace MangaReader_MVVM.Models
{
    public interface IChapter : IBindable, IComparable<IChapter>, IEquatable<IChapter>
    {
        Manga ParentManga { get; set; }
        float Number { get; set; }
        string Title { get; set; }
        string Id { get; set; }
        DateTime Released { get; set; }
        bool IsRead { get; set; }
        ObservableItemCollection<Page> Pages { get; set; }
    }
}
