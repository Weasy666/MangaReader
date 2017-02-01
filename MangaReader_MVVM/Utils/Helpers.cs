using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader_MVVM.Utils
{
    public static class Helpers
    {
        public static char CategorizeAlphabetically(char toCategorize)
        {
            if ('0' <= toCategorize && toCategorize <= '9')
                return '#';
            else if ('A' <= toCategorize && toCategorize <= 'Z')
                return toCategorize;
            else
                return '&';
        }
    }
}
