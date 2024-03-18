using CinemaManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public class MovieViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Movie> MoviesList { get; set; }
        public Movie SelectedMovie { get; set; }
        private ICollection<Genre> GenreList { get; set; }
        private ICollection<AgeCertificate> ageCertificates { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        //public RelayCommand EditCommand { get; set; }

        public MovieViewModel()
        {
            GenerateGenreData();
            GenerateAgeCertificate();
            MoviesList = GenerateSampleData();
            SelectedMovie = MoviesList[1];
            //DeleteCommand = new RelayCommand(execute => OnDelete(SelectedMovie), canExecute => SelectedMovie != null);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);

        }



        private bool CanDelete(object parameter)
        {
            // Add your condition here to determine if the delete command can execute
            return SelectedMovie != null;
        }

        private void OnDelete(object obj)
        {
            // Print a debug message to indicate that the OnDelete method is being called
            Debug.WriteLine("OnDelete method called.");

            // Check if SelectedMovie is correctly set
            if (SelectedMovie != null)
            {
                Debug.WriteLine($"Deleting movie: {SelectedMovie.Title}");

                // Remove the selected movie from the MoviesList
                MoviesList.Remove(SelectedMovie);
            }
            else
            {
                // Print a debug message if SelectedMovie is null
                Debug.WriteLine("SelectedMovie is null. Cannot delete.");
            }

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void GenerateGenreData()
        {
            if (GenreList == null)
            {
                GenreList = new List<Genre>
                {
                    new Genre { GenreId = 1, GenreName = "Action" },
                    new Genre { GenreId = 2, GenreName = "Adventure" },
                    new Genre { GenreId = 3, GenreName = "Comedy" },
                };
            }
        }
        private void GenerateAgeCertificate()
        {
            if (ageCertificates == null)
            {
                ageCertificates = new List<AgeCertificate>
                {
                    new AgeCertificate { AgeCertificateId = 1, DisplayContent = "C13", RequireAge = 13, ForegroundColor = "Orange", BackgroundColor = "Transparent"},
                    new AgeCertificate { AgeCertificateId = 2, DisplayContent = "C18", RequireAge = 18, ForegroundColor = "Red", BackgroundColor = "Transparent"},
                    new AgeCertificate { AgeCertificateId = 3, DisplayContent = "P", RequireAge = 0, ForegroundColor = "Green", BackgroundColor = "Transparent"},
                };
            }
        }

        private ObservableCollection<Movie> GenerateSampleData()
        {

            List<Genre> avatarGenres = new List<Genre>
            {
                GenreList.FirstOrDefault(g => g.GenreId == 1), // Action
                GenreList.FirstOrDefault(g => g.GenreId == 2), // Adventure
                GenreList.FirstOrDefault(g => g.GenreId == 3)  // Comedy
            };

            List<Genre> duneGenres = new List<Genre>
            {
                GenreList.FirstOrDefault(g => g.GenreId == 1), // Action
                GenreList.FirstOrDefault(g => g.GenreId == 2), // Adventure
            };
            List<Genre> pandaGenres = new List<Genre>
            {
                GenreList.FirstOrDefault(g => g.GenreId == 2), // Adventure
            };

            // Generate sample data for G
            ObservableCollection<Movie> res = new ObservableCollection<Movie>();
            res.Add(new Movie
            {
                Title = "Avatar: The way of water",
                Duration = 120,
                PublishYear = 2022,
                Imdbrating = 7.8,
                AgeCertificateId = 1,
                AgeCertificate = ageCertificates.ElementAt(0),
                PosterPath = "/Assets/Images/Poster/avatar_the_way_of_water.jpg",
                Description = "\"Avatar: The Way of Water\" is a sequel to the first \"Avatar\" film, set more than a decade after the events of the first film1. The story follows the Sully family (Jake, Neytiri, and their kids) as they seek refuge with the aquatic Metkayina clan of Pandora, a habitable exomoon on which they live2",
                TrailerPath = "/Assets/Videos/avatar.mp4",
                Genres = avatarGenres
                
            });
            res.Add(new Movie
            {
                Title = "Dune Part Two",
                Duration = 120,
                PublishYear = 2022,
                Imdbrating = 7.8,
                AgeCertificateId = 2,
                AgeCertificate = ageCertificates.ElementAt(1),
                PosterPath = "/Assets/Images/Poster/dune2.jpg",
                Description = "Dune 2 is the sequel to Dune (2021)1. It is the second of a two-part adaptation of the 1965 novel Dune by Frank Herbert1. The movie follows Paul Atreides as he unites with the Fremen people of the desert planet Arrakis to wage war against House Harkonnen1",
                TrailerPath = "/Assets/Videos/dune2.mp4",
                Genres = duneGenres

            });
            res.Add(new Movie
            {
                Title = "Avatar: The way of water",
                Duration = 120,
                PublishYear = 2022,
                Imdbrating = 7.8,
                AgeCertificateId = 1,
                AgeCertificate = ageCertificates.ElementAt(0),
                PosterPath = "/Assets/Images/Poster/avatar_the_way_of_water.jpg",
                Description = "\"Avatar: The Way of Water\" is a sequel to the first \"Avatar\" film, set more than a decade after the events of the first film1. The story follows the Sully family (Jake, Neytiri, and their kids) as they seek refuge with the aquatic Metkayina clan of Pandora, a habitable exomoon on which they live2",
                TrailerPath = "/Assets/Videos/avatar.mp4",
                Genres = pandaGenres

            });

            return res;
        }



    }
    public class GenreViewConverter : IValueConverter
    {
        public class CombinedItem
        {
            public String GenreName { get; set; }
            public String Icon { get; set; }
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is List<Genre> genres)
            {
                var combinedItems = new List<CombinedItem>();
                for (int i = 0; i < genres.Count; i++)
                {
                    combinedItems.Add(new CombinedItem
                    {
                        GenreName = genres[i].GenreName,
                        Icon = (i < genres.Count - 1) ? "\uF83F" : string.Empty
                    });
                }
                return combinedItems;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TotalMovieConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is ObservableCollection<Movie> movies)
            {
                int total = 0;
                total = movies.Count;
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
