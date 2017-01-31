using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Models
{
    public class MangaGroups : ObservableCollection<IManga>
    {
        public char Initial;
    }
}
