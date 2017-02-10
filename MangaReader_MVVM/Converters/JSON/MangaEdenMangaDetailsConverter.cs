using MangaReader_MVVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Template10.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MangaReader_MVVM.Converters.JSON
{
    class MangaEdenMangaDetailsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Manga));
        }

        // TODO: cleaner, maybe more generic and performant way to deserialize manga
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            JObject jo = JObject.Load(reader);
            
            // Construct the Result object using the non-default constructor
            var manga = new Manga
            {
                Title = System.Net.WebUtility.HtmlDecode(jo["title"].ToString()),
                Alias = System.Net.WebUtility.HtmlDecode(string.Join(", ", jo["aka"].AsEnumerable().Select(item => item.ToString()).ToArray())),
                Cover = new BitmapImage(new Uri(new Uri("https://cdn.mangaeden.com/mangasimg/"), jo["image"].ToString())),
                Category = string.Join(", ", jo["categories"].AsEnumerable().Select(item => item.ToString()).ToArray()),
                Author = jo["author"].ToString(),
                Artist = jo["artist"].ToString(),
                Description = System.Net.WebUtility.HtmlDecode(jo["description"].ToString()),
                Hits = (int)jo["hits"],
                Released = DateTimeOffset.FromUnixTimeSeconds(jo["created"] != null ? (long)jo["created"] : 0).DateTime.ToLocalTime(),
                LastUpdated = DateTimeOffset.FromUnixTimeSeconds(jo["last_chapter_date"] != null ? (long)jo["last_chapter_date"] : 0).DateTime.ToLocalTime(),
                Ongoing = (int)jo["status"] == 1,
                NumberOfChapters = (int)jo["chapters_len"],
            };

            var chapters = jo["chapters"].AsEnumerable().Select(chapter => new Chapter
            {
                Number = (float)chapter[0],
                Title = chapter[2].ToString().Any() ? chapter[2].ToString() : ((float)chapter[0]).ToString(),
                Id = chapter[3].ToString(),
                Released = DateTimeOffset.FromUnixTimeSeconds(chapter[1] != null ? (long)chapter[1] : 0).DateTime.ToLocalTime(),
                IsRead = false
            }).ToList();
            chapters.Sort();
            manga.Chapters = new ObservableItemCollection<Chapter>(chapters);

            // Return the result
            return manga;
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}