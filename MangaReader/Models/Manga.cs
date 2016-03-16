using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MangaReader.Models
{
    public class Manga
    {
        public string Title { get; set; }
        public List<Chapter> Chapters { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public string Source { get; set; }

        public class Chapter
        {
            public int Number { get; set; }
            public string Source { get; set; }
        }
    }
}
