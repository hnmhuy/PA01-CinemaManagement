using CinemaManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    public class SearchPageViewModel : INotifyPropertyChanged
    {
        public Movie HighlightingMovie = null;
        private DbCinemaManagementContext db = new DbCinemaManagementContext();
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                Debug.WriteLine("Search text" + _searchText);
                OnPropertyChanged(nameof(SearchText));
            }
        }
        private bool isSearching { get; set; }
        public bool IsSearching
        {
            get { return isSearching; }
            set
            {
                isSearching = value;
                OnPropertyChanged(nameof(IsSearching));
            }
        }
        private bool isSearchResultEmpty
        {
            get { return SearchResults.Count == 0; }
        }
        public bool IsSearchResultEmpty
        {
            get { return isSearchResultEmpty; }
            set
            {
                OnPropertyChanged(nameof(IsSearchResultEmpty));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Override the OnPropertyChanged method
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Movie> SearchResults { get; set; } = new ObservableCollection<Movie>();
        

        public SearchPageViewModel()
        {
            // Add an sample data
            //var temp = db.Movies.ToList();
            //foreach (var item in temp)
            //{
            //    SearchResults.Add(item);
            //}
            IsSearching = false;
            IsSearchResultEmpty = false;
        }

        public void Search()
        {
            IsSearching = true;
            IsSearchResultEmpty = false;
            SearchResults.Clear();
            var temp = db.Movies.Where(x => x.Title.Contains(SearchText)).ToList();
            foreach (var item in temp)
            {
                item.AgeCertificate = db.AgeCertificates.Find(item.AgeCertificateId);
                SearchResults.Add(item);
            }


            // Try to search by contributors
            if (SearchResults.Count == 0)
            {
                var contributors = db.People.Where(x => x.Fullname.Contains(SearchText)).ToList();
                foreach (var contributor in contributors)
                {
                    var movies = db.Contributors.Include(x => x.Movie).Where(x => x.PersonId == contributor.PersonId).Select(x => x.Movie).ToList();
                    foreach (var movie in movies)
                    {
                        movie.AgeCertificate = db.AgeCertificates.Find(movie.AgeCertificateId);
                        // Check if the movies is already in the search results
                        if (!SearchResults.Contains(movie))
                            SearchResults.Add(movie);
                    }
                }
            }

            //// Try to search by genres
            

            //foreach (var movie in moviesByGenres)
            //{
            //    movie.AgeCertificate = db.AgeCertificates.Find(movie.AgeCertificateId);
            //    if (!SearchResults.Contains(movie))
            //        SearchResults.Add(movie);
            //}

            IsSearching = false;
            if (SearchResults.Count == 0)
            {
                IsSearchResultEmpty = true;
            }
        }

    }
}
