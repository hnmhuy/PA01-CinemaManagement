using CinemaManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaManagement.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Xaml.Data;
using Microsoft.EntityFrameworkCore;
using CinemaManagement.Views;
namespace CinemaManagement.ViewModels
{
    class DashboardViewModel
    {
        public class MovieWrapper
        {
            public int Rank { get; set; }
            public double revenue { get; set; }
            public Movie Movie { get; set; }

            public MovieWrapper(Movie movie, int rank, double revenue)
            {
                this.Movie = movie;
                this.Rank = rank;
                this.revenue = revenue;
            }
        }
        public ObservableCollection<MovieWrapper> TopHighestGrossingMovies { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int _movieOnSale;
        public int MovieOnSale
        {
            get { return _movieOnSale; }
            set 
            { 
                _movieOnSale = value;
                OnPropertyChanged(nameof(MovieOnSale));
            }
        }

        public int _showTimesToday;
        public int ShowTimesToday
        {
            get { return _showTimesToday; }
            set
            {
                _showTimesToday = value;
                OnPropertyChanged(nameof(ShowTimesToday));
            }
        }

        public int _showTimesThisWeek;
        public int ShowTimesThisWeek
        {
            get { return _showTimesThisWeek; }
            set
            {
                _showTimesThisWeek = value;
                OnPropertyChanged(nameof(ShowTimesThisWeek));
            }
        }

        public int _showTimesThisMonth;
        public int ShowTimesThisMonth
        {
            get { return _showTimesThisMonth; }
            set
            {
                _showTimesThisMonth = value;
                OnPropertyChanged(nameof(ShowTimesThisMonth));
            }
        }

        public ObservableCollection<MovieWrapper> MoviesList { get; set; }
        private ICollection<Genre> GenreList { get; set; }
        private ICollection<AgeCertificate> ageCertificates { get; set; }

        private ICollection<String> Rank { get; set; }
        public DbCinemaManagementContext db = new DbCinemaManagementContext();
        public DashboardViewModel()
        {
            MovieOnSale= CountMovieOnSale();
            ShowTimesToday = CountShowTimesToday();
            ShowTimesThisWeek = CountShowTimesThisWeek();
            ShowTimesThisMonth = CountShowTimesThisMonth();
            TopHighestGrossing();
        }

        private void TopHighestGrossing()
        {
            if(TopHighestGrossingMovies == null)
            {
                TopHighestGrossingMovies = new ObservableCollection<MovieWrapper>();
            }

            var showTimes = db.ShowTimes
                   .Include(st => st.Movie)
                   .Include(st => st.Tickets)
                   .ToList();  // Load data into memory

            var topMovies = showTimes
                            .Where(st => st.Movie != null)
                            .GroupBy(st => new { st.MovieId, st.Movie.Title, st.Movie.Duration, st.Movie.PublishYear, st.Movie.PosterPath, Genres = string.Join(", ", st.Movie.Genres.Select(g => g.GenreName)), st.Movie.Imdbrating })
                            .Select(g => new
                            {
                                g.Key.MovieId,
                                g.Key.Title,
                                g.Key.PosterPath,
                                g.Key.Duration,
                                g.Key.PublishYear,
                                g.Key.Genres,
                                g.Key.Imdbrating,
                                Revenue = g.Sum(st => st.Tickets.Where(t => t.IsAvailable == false).Sum(t => t.Price))
                            })
                            .OrderByDescending(m => m.Revenue)
                            .Take(10)
                            .ToList();

            for (int i = 0; i < topMovies.Count; i++)
            {
                TopHighestGrossingMovies.Add(
                    new MovieWrapper(
                        new Movie { 
                            Title = topMovies[i].Title,
                            Duration = topMovies[i].Duration,
                            PublishYear = topMovies[i].PublishYear,
                            Imdbrating = topMovies[i].Imdbrating,
                            PosterPath = topMovies[i].PosterPath,
                    }
                ,i + 1, topMovies[i].Revenue));
            }
        }

        private int CountShowTimesThisMonth()
        {
            int thisMonth = DateTime.Now.Month;
            int thisYear = DateTime.Now.Year;

            var count = db.ShowTimes
                               .Where(st => st.ShowDate.Month == thisMonth && st.ShowDate.Year == thisYear)
                               .Count();
            return count;
        }

        private int CountShowTimesToday()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            var count = db.ShowTimes
                               .Where(st => st.ShowDate >= today && st.ShowDate < tomorrow)
                               .Count();
            return count;
        }

        private int CountShowTimesThisWeek()
        {
            DateTime selectedDate = DateTime.Now;
            int daysUntilMonday = (int)selectedDate.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysUntilMonday < 0)
            {
                daysUntilMonday += 7;
            }

            DateTime startDate = selectedDate.AddDays(-daysUntilMonday);
            DateTime endDate = selectedDate.AddDays(8);

            var count = db.ShowTimes
                               .Where(st => st.ShowDate >= startDate && st.ShowDate < endDate)
                               .Count();
            return count;
        }

        private int CountMovieOnSale()
        {
            int count = db.ShowTimes
                       .Where(st => st.ShowDate >= DateTime.Now)
                       .Select(st => st.MovieId)
                       .Distinct()
                       .Count();
            return count;
        }
    }
}
