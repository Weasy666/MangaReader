using System;
using System.Collections.Generic;
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
        private string root;
        private string api;
        private static HttpClient HttpClient { get; set; }
        private static Rootobject Repository { get; set; }

        public MangaEdenRepository()
        {
            root = "http://www.mangaeden.com";
            api = string.Format("/api/list/{0}/", language);
            HttpClient = new HttpClient();
            HttpClient.BaseAddress = new Uri(root);
        }

        public async void LoadManga()
        {
            var response = await HttpClient.GetAsync(api);
            var result = await response.Content.ReadAsStringAsync();
            Repository = JsonConvert.DeserializeObject<Rootobject>(result);
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

    public class MangaEdenManga
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
    }
}
