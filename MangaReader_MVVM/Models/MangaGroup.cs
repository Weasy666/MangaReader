using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    public class MangaGroup : ObservableCollection<Manga>
    {
        public char Initial { get; set; }

        public override string ToString()
        {
            return Initial.ToString();
        }
    }
}
