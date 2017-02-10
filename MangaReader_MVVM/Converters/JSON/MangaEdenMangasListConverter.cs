using MangaReader_MVVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Template10.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Converters.JSON
{
    class MangaEdenMangasListConverter : JsonConverter
    {
        private MangaSource _source;
        public MangaEdenMangasListConverter(MangaSource source)
        {
            _source = source;
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ObservableItemCollection<Manga>));
        }

        // TODO: cleaner, maybe more generic and performant way to deserialize mangaList
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            JToken jo = JObject.Load(reader).GetValue("manga");

            // Construct the Result object using the non-default constructor
            var mangas = jo.Select(manga => new Manga
            {
                MangaSource = _source,
                Title = System.Net.WebUtility.HtmlDecode(manga["t"].ToString()),
                Alias = System.Net.WebUtility.HtmlDecode(manga["a"].ToString()),
                Id = manga["i"].ToString(),
                Cover = new BitmapImage(new Uri(new Uri("https://cdn.mangaeden.com/mangasimg/"), manga["im"].ToString())),
                Category = string.Join(", ", manga["c"].AsEnumerable().Select(item => item.ToString()).ToArray()),
                Hits = (int)manga["h"],
                LastUpdated = DateTimeOffset.FromUnixTimeSeconds(manga["ld"] != null ? (long)manga["ld"] : 0).DateTime.ToLocalTime(),
                Ongoing = (int)manga["s"] != 2, // 0 = Suspended, 1 = Ongoing, 2 = Finished
                IsFavorit = false
            }).ToList();
            mangas.Sort();

            // Return the result
            return new ObservableItemCollection<Manga>(mangas);
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}