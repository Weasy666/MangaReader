using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static string Root { get; }
        public static string ApiAllManga { get; }
        public static string ApiMangaInfos { get; }
        public static string ApiMangaChapterPages { get; }
        private MangaList Repository { get; set; }

        static MangaEdenRepository()
        {
            Root = "http://www.mangaeden.com";
            ApiAllManga = string.Format("/api/list/{0}/", Language);
            ApiMangaInfos = "/api/manga/{0}/";
            ApiMangaChapterPages = "/api/chapter/{0}";
        }

        /// <summary>
        /// Load the complete list of manga from MangaEden
        /// </summary>
        public async Task LoadManga()
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Root) })
            {
                try
                {
                    var response = await httpClient.GetAsync(ApiAllManga);
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
        public ObservableCollection<Manga> GetListOfManga()
        {
            var mangaEdenMangas = Repository.manga.ToList();
            var mangas = mangaEdenMangas.Select(manga => new Manga
            {
                Title = System.Net.WebUtility.HtmlDecode(manga.Title),
                Alais = System.Net.WebUtility.HtmlDecode(manga.a),
                Id = manga.i,
                MangaCover = manga.Image,
                Category = manga.Category,
                Hits = manga.h,
                LastUpdated = manga.LastUpdated,
                Status = manga.Status
            }).ToList();
            mangas.Sort();
            return new ObservableCollection<Manga>(mangas);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<Manga> LoadInfosAsync(Manga manga)
        {
            MangaEdenMangaInfos mangaInfos = null;
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Root) })
            {
                try
                {
                    var response = await httpClient.GetAsync(string.Format(ApiMangaInfos, manga.Id));
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        mangaInfos = JsonConvert.DeserializeObject<MangaEdenMangaInfos>(result);
                    }
                }
                catch (HttpRequestException e)
                {
                    var dialog = new MessageDialog(e.Message);
                    await dialog.ShowAsync();
                }
            }
            if (mangaInfos != null)
            {
                manga.Artist = System.Net.WebUtility.HtmlDecode(mangaInfos.artist);
                manga.Author = System.Net.WebUtility.HtmlDecode(mangaInfos.author);
                manga.Chapters = await mangaInfos.GetChaptersAsListAsync();
                manga.Description = System.Net.WebUtility.HtmlDecode(mangaInfos.description);
                manga.NumberOfChapters = mangaInfos.chapters_len;
                manga.Released = mangaInfos.released.ToString();
            }
            return manga;
        }

        public async Task<ObservableCollection<MangaPage>> LoadPagesAsync(Chapter chapter)
        {
            MangaEdenChapterPages chapterPages = null;
            using (var httpClient = new HttpClient { BaseAddress = new Uri(Root) })
            {
                try
                {
                    var response = await httpClient.GetAsync(string.Format(ApiMangaChapterPages, chapter.Id));
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        chapterPages = JsonConvert.DeserializeObject<MangaEdenChapterPages>(result);
                    }
                }
                catch (HttpRequestException e)
                {
                    var dialog = new MessageDialog(e.Message);
                    await dialog.ShowAsync();
                }
            }
            if (chapterPages != null)
            {
                var pages = chapterPages.GetPagesAsList();
                pages.Sort();
                return new ObservableCollection<MangaPage>(pages);
            }
            return new ObservableCollection<MangaPage>();
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
        public DateTime LastUpdated => DateTimeOffset.FromUnixTimeSeconds((long)ld).DateTime.ToLocalTime(); //ToString(CultureInfo.CurrentCulture);

        public int CompareTo(MangaEdenManga comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : this.t.CompareTo(comparePart.t);
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

        public async Task<ObservableCollection<Chapter>> GetChaptersAsListAsync()
        {
            var list = chapters.Select(chap => new Chapter
            {
                Number = chap[0].ToString(),
                Released = DateTimeOffset.FromUnixTimeSeconds(long.Parse(chap[1].ToString())).DateTime.ToLocalTime(),
                Title = chap[2] != null ? System.Net.WebUtility.HtmlDecode(chap[2].ToString()) : chap[0].ToString(),
                Id = chap[3].ToString(),
            }).ToList();
            list.Sort();
            //foreach (var chap in list)
            //{
            //    var buffer = await LoadPages(chap.Id);
            //    chap.Pages = buffer;
            //    chap.ChapterCover = buffer[0].Url;
            //}
            return new ObservableCollection<Chapter>(list);
        }

        //private static async Task<ObservableCollection<MangaPage>> LoadPages(Chapter chapter)
        //{
        //    MangaEdenChapterPages chapterPages = null;
        //    using (var httpClient = new HttpClient { BaseAddress = new Uri(MangaEdenRepository.Root) })
        //    {
        //        try
        //        {
        //            var response = await httpClient.GetAsync(string.Format(MangaEdenRepository.ApiMangaChapterPages, chapter.Id));
        //            var result = await response.Content.ReadAsStringAsync();
        //            if (response.IsSuccessStatusCode)
        //            {
        //                chapterPages = JsonConvert.DeserializeObject<MangaEdenChapterPages>(result);
        //            }
        //        }
        //        catch (HttpRequestException e)
        //        {
        //            var dialog = new MessageDialog(e.Message);
        //            await dialog.ShowAsync();
        //        }
        //    }
        //    if (chapterPages != null)
        //        return chapterPages.GetPagesAsList();
        //    return new ObservableCollection<MangaPage>();
        //}
    }

    public class MangaEdenChapterPages
    {
        public object[][] images { get; set; }

        public List<MangaPage> GetPagesAsList()
        {
            return images.Select(page => new MangaPage
            {
                Number = page[0].ToString(),
                Url = "https://cdn.mangaeden.com/mangasimg/" + page[1].ToString(),
                Width = int.Parse(page[2].ToString()),
                Height = int.Parse(page[3].ToString())
            }).ToList();
        }
    }
}
