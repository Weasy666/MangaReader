using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader.Controls
{
    public class MangaManager
    {
        public MangaScrapeLib.Repositories.MangaFoxRepository MangaFoxRepo { get; }

        public MangaManager()
        {
            MangaFoxRepo = new MangaScrapeLib.Repositories.MangaFoxRepository();
        }
    }
}
