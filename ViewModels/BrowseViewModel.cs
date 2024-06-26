﻿using CinemaManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    public class RankMovie
    {
        public int Rank { get; set; }
        public Movie Movie { get; set; }
        public RankMovie(int rank, Movie movie)
        {
            Rank = rank;
            Movie = movie;
        }
    }

    public class BrowseViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Movie> GoldenHours { get; set; }
        public ObservableCollection<Movie> NewRelease { get; set; }
        public ObservableCollection<Movie> BlockBuster { get; set; }
        public ObservableCollection<Movie> MovieOnSale { get; set; }
        public ObservableCollection<RankMovie> TopTen { get; set; }
        public Movie PannelMovie { get; set; }
        public DbCinemaManagementContext db = new DbCinemaManagementContext();
        private bool _isLoaded = false;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                _isLoaded = value;
                OnPropertyChanged(nameof(IsLoaded));
            }
        }
        
        public Movie HighlightingMovie = null;

        public event PropertyChangedEventHandler PropertyChanged;

        // Override the OnPropertyChanged method
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ICollection<Genre> GenreList { get; set; }

        private ICollection<AgeCertificate> ageCertificates { get; set; }

        public BrowseViewModel()
        {
            GoldenHours = new ObservableCollection<Movie>();
            NewRelease = new ObservableCollection<Movie>();
            BlockBuster = new ObservableCollection<Movie>();
            TopTen = new ObservableCollection<RankMovie>();
            IsLoaded = false;
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

        private ObservableCollection<RankMovie> GenerateTopTen()
        {
            ObservableCollection<RankMovie> res = new ObservableCollection<RankMovie>();
            for (int i = 1; i <= 10; i++) { 
                res.Add(new RankMovie(i, GoldenHours.ElementAt(0)));
            }
            return res;
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
                TrailerPath = "/Assets/Videos/avatar.mp4"
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
                TrailerPath = "/Assets/Videos/dune2.mp4"
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
                TrailerPath = "/Assets/Videos/avatar.mp4"
            });

            return res;
        }

        public void LoadData()
        {
            IsLoaded = false;
            // Await to load data from database
            // Get the newest movies from database for PannelMovie base on added time
            
            //PannelMovie = db.Movies.OrderByDescending(m => m.AddedTime).FirstOrDefault();


            LoadGoldenHours();
            LoadBlockBuster();
            LoadMovieOnSale();
            LoadTopTen();
            IsLoaded = true;
        }   

        private void LoadTopTen()
        {
            // Load top ten movies from database
            if (TopTen == null)
            {
                TopTen = new ObservableCollection<RankMovie>();
            }

            var topTenQuer = from st in db.ShowTimes
                             join t in db.Tickets on st.ShowTimeId equals t.ShowTimeId
                             join m in db.Movies on st.MovieId equals m.MovieId
                             where !t.IsAvailable
                             group m by m into g
                             orderby g.Count() descending
                             select g.Key;

            var temp = topTenQuer.Take(10).ToList();

            for (int i = 0; i < temp.Count; i++)
            {
                // Update the AgeCertificate
                temp[i].AgeCertificate = db.AgeCertificates.Find(temp[i].AgeCertificateId);
                TopTen.Add(new RankMovie(i + 1, temp[i]));
               
            }
        }

        private void LoadGoldenHours()
        {
            var temp = db.Movies.Where(m => m.IsGoldenHour == true).OrderBy(m => m.PublishYear).ToList();
            if (GoldenHours == null)
            {
                GoldenHours = new ObservableCollection<Movie>();
            }
            ConvertFromListToObservableCollection(temp, GoldenHours);
            
        }

        private void LoadMovieOnSale()
        {
            var temp = db.Movies.Where(m => !m.IsGoldenHour && !m.IsBlockbuster).OrderBy(m => m.PublishYear).ToList();
            if (MovieOnSale == null)
            {
                MovieOnSale = new ObservableCollection<Movie>();
            }
            ConvertFromListToObservableCollection(temp, MovieOnSale);
            
        }

        private void LoadBlockBuster()
        {
            // Load blockbuster movies from database
            if (BlockBuster == null)
            {
                BlockBuster = new ObservableCollection<Movie>();
            }
            var temp = db.Movies.Where(m => m.IsBlockbuster == true).OrderBy(m => m.PublishYear).ToList();
            ConvertFromListToObservableCollection(temp, BlockBuster);
        }

        private void ConvertFromListToObservableCollection(List<Movie> src, ObservableCollection<Movie> des)
        {
            // Convert list to observable collection
            for (int i = 0; i < src.Count; i++)
            {
                src[i].AgeCertificate = db.AgeCertificates.Find(src[i].AgeCertificateId);
                des.Add(src[i]);
            }
        }

    }


    public class RankIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int rank  = (int)value;
            if (rank >=1 && rank <=9)
            {
                return "ms-appx:///Assets/Icons/Rank0" + rank + ".png";
            } else
            {
                return "ms-appx:///Assets/Icons/Rank10.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvertFullPath : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Project path
            string refix = parameter as string;
            string res =  "ms-appx://"  + value;
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
