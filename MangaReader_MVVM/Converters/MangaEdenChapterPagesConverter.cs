using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    class MangaEdenChapterPagesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ObservableCollection<IPage>));
        }

        // TODO: cleaner, maybe more generic and performant way to deserialize manga
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            var jo = JObject.Load(reader).GetValue("images");

            // Construct the Result object using the non-default constructor
            var pages = jo.Select(page => new Page
            {
                Number = (int)page[0],
                Url = new BitmapImage(new Uri(new Uri("https://cdn.mangaeden.com/mangasimg/"), page[1].ToString())),
                Width = (int)page[2],
                Height = (int)page[3]
            }).ToList();
            pages.Sort();

            return new ObservableCollection<IPage>(pages);
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}