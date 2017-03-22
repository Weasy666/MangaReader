﻿using MangaReader_MVVM.Models;
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

        public static void SetTitlebarText(Manga manga)
        {
            var currentView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            if (manga != null)
            {
                currentView.Title = manga.Title;
            }
            else
            {
                currentView.Title = "";
            }
        }

        public static void SetTitlebarText(Chapter chapter)
        {
            var currentView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            if (chapter != null)
            {
                currentView.Title = chapter.ParentManga.Title + (chapter.Title == chapter.Number.ToString() ? "" : (" - " + chapter.Number)) + " - " + chapter.Title;
            }
            else
            {
                currentView.Title = "";
            }
        }

        public static void SetTitlebarText(string title)
        {
            var currentView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            if (title != null)
            {
                currentView.Title = title;
            }
            else
            {
                currentView.Title = "";
            }
        }
    }
}
