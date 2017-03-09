using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM
{
    public enum MangaSource
    {
        MangaEden,
        //MangaFox,
        //MangaReader
    }
    
    public enum ReloadMode
    {
        Local,
        Server
    }

    public enum MangaItemTemplate
    {
        CoverOnly,
        CoverWithDetails
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

    public enum ReadDirection
    {
        [Display(Name = "Left to Right")]
        LeftToRight,
        [Display(Name = "Right to Left")]
        RightToLeft
    }

    public enum StorageStrategies
    {
        Local,
        OneDrive,
        Roaming,
        Temporary
    }
}
