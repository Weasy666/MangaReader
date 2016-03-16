using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;

namespace MangaReader.Models
{
    public class MangaEdenRepository
    {
        //MangaEden languages are: english(0) and italian(1)
        private static int language = 0;
        private static string root;
        private static string apiAllManga;
        protected static string apiMangaInfos;
        protected static HttpClient HttpClient { get; set; }
        private static Rootobject Repository { get; set; }

        static MangaEdenRepository()
        {
            root = "http://www.mangaeden.com";
            apiAllManga = string.Format("/api/list/{0}/", language);
            apiMangaInfos = "/api/manga/{0}/";
            HttpClient = new HttpClient {BaseAddress = new Uri(root)};
        }

        public async Task<Rootobject> LoadManga()
        {
            var response = await HttpClient.GetAsync(apiAllManga);
            var result = await response.Content.ReadAsStringAsync();
            return Repository = JsonConvert.DeserializeObject<Rootobject>(result);
        }

        public List<MangaEdenManga> GetManga()
        {
            return Repository.manga.ToList();
        }
    }

    public class Rootobject
    {
        public int end { get; set; }
        public MangaEdenManga[] manga { get; set; }
        public int page { get; set; }
        public int start { get; set; }
        public int total { get; set; }
    }

    public class MangaEdenManga : MangaEdenRepository
    {
        //Alias
        public string a { get; set; }
        //Array of Categories
        public string[] c { get; set; }
        //Hits
        public int h { get; set; }
        //ID
        public string i { get; set; }
        //Image URL
        public string im { get; set; }
        //Last Chapter Date
        public float ld { get; set; }
        //Status
        public int s { get; set; }
        //Title of Manga
        public string t { get; set; }
        //Renaming for consistent Binding between different Sources
        public string Image => "https://cdn.mangaeden.com/mangasimg/" + im;
        public string Title => t;
        public string Category => string.Join(", ", c);
        public string Status => s != 0 ? "Ongoing" : "Completed";
        public string LastUpdated => ld.ToString();
        //Mangainfos and Chapters
        public MangaEdenMangaInfos MangaInfos { get; set; }

        public int NumberOfChapters => MangaInfos.chapters_len;

        public async void LoadInfos()
        {
            var response = await HttpClient.GetAsync(string.Format(apiMangaInfos, i));
            var result = await response.Content.ReadAsStringAsync();
            MangaInfos = JsonConvert.DeserializeObject<MangaEdenMangaInfos>(result);
        }
    }

    public class MangaEdenMangaInfos
    {
        public object[] aka { get; set; }
        public object[] akaalias { get; set; }
        public string alias { get; set; }
        public string artist { get; set; }
        public object[] artist_kw { get; set; }
        public string author { get; set; }
        public object[] author_kw { get; set; }
        public string[] categories { get; set; }
        public object[][] chapters { get; set; }
        public int chapters_len { get; set; }
        public float created { get; set; }
        public string description { get; set; }
        public int hits { get; set; }
        public string image { get; set; }
        public int language { get; set; }
        public float last_chapter_date { get; set; }
        public float[] random { get; set; }
        public object released { get; set; }
        public string startsWith { get; set; }
        public int status { get; set; }
        public string title { get; set; }
        public string[] title_kw { get; set; }
        public int type { get; set; }
        public bool updatedKeywords { get; set; }
    }

    public class Chapter
    {
        public int Number { get; set; }
        public float Date { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
    }
}
