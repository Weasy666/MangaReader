using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using AngleSharp;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;

namespace MangaReader.Models
{
    public class MangaEdenRepository
    {
        //MangaEden languages are: english(0) and italian(1)
        private static int Language = 0;
        protected static string Root;
        private static string ApiAllManga;
        protected static string ApiMangaInfos;
        private MangaList Repository { get; set; }

        static MangaEdenRepository()
        {
            Root = "http://www.mangaeden.com";
            ApiAllManga = string.Format("/api/list/{0}/", Language);
            ApiMangaInfos = "/api/manga/{0}/";
        }

        /// <summary>
        /// Load the complete list of manga
        /// </summary>
        public async Task LoadManga()
        {
            using (var testClient = new HttpClient {BaseAddress = new Uri(Root)})
            {
                try
                {
                    var response = await testClient.GetAsync(ApiAllManga);
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        Repository = JsonConvert.DeserializeObject<MangaList>(result);
                    }
                }
                catch (HttpRequestException e)
                {
                    var dialog = new MessageDialog(e.Message);
                    await dialog.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Returns an alphabetically sorted List of MangaEdenManga
        /// </summary>
        /// <returns></returns>
        public List<Manga> GetManga()
        {
            var mangaEdenMangas = Repository.manga.ToList();
            var mangas = new List<Manga>();
            foreach (var manga in mangaEdenMangas)
            {
                var toAdd = new Manga
                {
                    Title = manga.Title,
                    Alais = manga.a,
                    ID = manga.i,
                    ImgCover = manga.Image,
                    Category = manga.Category,
                    Hits = manga.h,
                    LastUpdated = manga.LastUpdated,
                    Status = manga.Status
                };
                mangas.Add(toAdd);
            }
            mangas.Sort();
            return mangas;
        }
    }

    public class MangaList
    {
        public int end { get; set; }
        public MangaEdenManga[] manga { get; set; }
        public int page { get; set; }
        public int start { get; set; }
        public int total { get; set; }
    }

    public class MangaEdenManga : IComparable<MangaEdenManga>
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
        public DateTime LastUpdated => DateTimeOffset.FromUnixTimeSeconds((long) ld).DateTime.ToLocalTime(); //ToString(CultureInfo.CurrentCulture);

        public int CompareTo(MangaEdenManga comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : this.t.CompareTo(comparePart.t);
        }

        //Mangainfos and Chapters
        //public MangaEdenMangaInfos MangaInfos { get; set; }

        //public int NumberOfChapters => MangaInfos.chapters_len;

        //public async void LoadInfos()
        //{
        //    using (HttpClient = new HttpClient { BaseAddress = new Uri(root) })
        //    {
        //        try
        //        {
        //            var response = await HttpClient.GetAsync(string.Format(apiMangaInfos, i));
        //            var result = await response.Content.ReadAsStringAsync();

        //            if (response.IsSuccessStatusCode)
        //            {
        //                MangaInfos = JsonConvert.DeserializeObject<MangaEdenMangaInfos>(result);
        //            }
        //        }
        //        catch (HttpRequestException e)
        //        {
        //            var dialog = new MessageDialog(e.Message);
        //            await dialog.ShowAsync();
        //        }
        //    }
        //}
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
        //Time of last update in UNIX Time stamp format
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
}
