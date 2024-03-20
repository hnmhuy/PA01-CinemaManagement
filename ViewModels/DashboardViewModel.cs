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
namespace CinemaManagement.ViewModels
{
    class DashboardViewModel
    {
        public class MovieWrapper
        {
            public String Rank { get; set; }
            public Movie Movie { get; set; }

            public MovieWrapper(Movie movie, String rank)
            {
                this.Movie = movie;
                this.Rank = rank;
            }
        }
        public ObservableCollection<MovieWrapper> MoviesList { get; set; }
        private ICollection<Genre> GenreList { get; set; }
        private ICollection<AgeCertificate> ageCertificates { get; set; }

        private ICollection<String> Rank { get; set; }
        public DashboardViewModel()
        {
            GenerateGenreData();
            GenerateAgeCertificate();
            MoviesList = GenerateSampleData();
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

        private ObservableCollection<MovieWrapper> GenerateSampleData()
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
            ObservableCollection<MovieWrapper> res = new ObservableCollection<MovieWrapper>();
            res.Add(new MovieWrapper(new Movie
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
            }, "/Assets/Icons/Rank01.png"));
            res.Add(new MovieWrapper(new Movie
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

            }, "/Assets/Icons/Rank02.png"));
            res.Add(new MovieWrapper(new Movie
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

            }, "/Assets/Icons/Rank03.png"));
            Console.WriteLine(res);
            return res;
        }
    


    }

}
