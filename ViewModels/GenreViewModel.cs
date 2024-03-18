using CinemaManagement.Models;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Forms;
using Windows.UI.Popups;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
namespace CinemaManagement.ViewModels
{

    public class GenreViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Genre> GenresList { get; set; }
        public Genre SelectedGenre { get; set; }

        public GenreViewModel()
        {
            GenresList = GenerateGenreSampleData();
            SelectedGenre = GenresList[1];
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection <Genre> GenerateGenreSampleData()
        {
            ObservableCollection<Genre> genres = new ObservableCollection<Genre>();
            genres.Add(
                new Genre
                {
                    GenreId = 1,
                    GenreName = "Sci-Fi"
                });
            genres.Add(
                new Genre
                {
                    GenreId = 2,
                    GenreName = "Action"
                });
            genres.Add(
                new Genre
                {
                    GenreId = 3,
                    GenreName = "K-Drama"
                });
            genres.Add(
                new Genre
                {
                    GenreId = 4,
                    GenreName = "Romance"
                });
            genres.Add(
                new Genre
                {
                    GenreId = 5,
                    GenreName = "Fantasy"
                });
            genres.Add(
                new Genre
                {
                    GenreId = 6,
                    GenreName = "Comedy"
                });
            return genres;
        }


    }
    public class TotalGenreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<Genre> genres)
            {
                int total = 0;
                total = genres.Count;
                return total;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
