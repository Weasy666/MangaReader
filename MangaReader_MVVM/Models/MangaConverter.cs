using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Models
{
    class MangaConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ObservableCollection<Manga>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            JToken jo = JObject.Load(reader).GetValue("manga");

            // Construct the Result object using the non-default constructor
            var mangas = jo.Select(manga => new Manga
            {
                Title = System.Net.WebUtility.HtmlDecode(manga["t"].ToString()),
                Alias = System.Net.WebUtility.HtmlDecode(manga["a"].ToString()),
                Id = manga["i"].ToString(),
                Cover = new BitmapImage(new Uri(manga["im"].ToString())),
                Category = manga["c"].ToString(),
                Hits = (int)manga["h"],
                LastUpdated = DateTimeOffset.FromUnixTimeSeconds((long)manga["ld"]).DateTime.ToLocalTime(),
                Ongoing = (int)manga["s"] == 1
            }).ToList();
            mangas.Sort(); 

            // (If anything else needs to be populated on the result object, do that here)

            // Return the result
            return new ObservableCollection<Manga>(mangas);
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
