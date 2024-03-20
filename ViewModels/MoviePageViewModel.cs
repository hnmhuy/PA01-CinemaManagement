using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    public class MoviePageViewModel : INotifyPropertyChanged
    {
        private MovieViewModel _moviesList;
        public MovieViewModel MoviesList
        {
            get { return _moviesList; }
            set
            {
                _moviesList = value;
                RaisePropertyChanged("MoviesList");
            }
        }

        private GenreViewModel _genresList;
        public GenreViewModel GenresList
        {
            get { return _genresList; }
            set
            {
                _genresList = value;
                RaisePropertyChanged("GenresList");
            }
        }

        public MoviePageViewModel()
        {
            MoviesList = new MovieViewModel();
            GenresList = new GenreViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
