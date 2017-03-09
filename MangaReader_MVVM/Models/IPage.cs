using MangaReader_MVVM.Converters.JSON;
using Newtonsoft.Json;
using System;
using Template10.Mvvm;
using Windows.UI.Xaml;

namespace MangaReader_MVVM.Models
{
    [JsonConverter(typeof(InterfaceConverter<IPage, Page>))]
    public interface IPage : IBindable, IComparable<IPage>
    {
        Visibility OverlayVisibility { get; set; }
        int Number { get; set; }
        Uri Url { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }
}
