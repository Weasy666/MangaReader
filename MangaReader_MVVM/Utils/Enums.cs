using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM
{
    public enum MangaSource
    {
        MangaEden,
        MangaFox,
        MangaReader
    }
    
    public enum ReloadMode
    {
        Default,
        FromSource
    }

    public enum ReadMode
    {
        [Display(Name = "Horizontal Continuous")]
        HorizontalContinuous,
        [Display(Name = "Horizontal Single Page")]
        HorizontalSingle,
        [Display(Name = "Vertical Continuous")]
        VerticalContinuous,
        [Display(Name = "Vertical Single Page")]
        VerticalSingle
    }
}
