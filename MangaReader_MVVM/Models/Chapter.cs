using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MangaReader_MVVM.Models
{
    public class Chapter : Template10.Mvvm.ViewModelBase, IChapter
    {
        public IManga ParentManga { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }        
        public int NumberOfPages => 0; //Pages.Count;
        public DateTime Released { get; set; }
        private bool _isRead = false;
        public bool IsRead
        {
            get
            {
                return _isRead;
            }
            set
            {
                _isRead = value;
                base.RaisePropertyChanged();
            }
        }
        public ObservableCollection<IPage> Pages { get; set; }

        public int CompareTo(Chapter comparePart)
        {
            // A null value means that this object is greater.
            return comparePart == null ? 1 : this.Number.CompareTo(comparePart.Number);
        }
    }
}
