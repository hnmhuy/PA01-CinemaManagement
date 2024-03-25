using CinemaManagement.Models;
using CinemaManagement.WindowViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Data;
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

    public class ShowtimeCommand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public ShowTime Showtime { get; set; }

        public int selectedIndex { get; set; }

        public ShowtimeCommand(ShowTime _Showtime, RelayCommand _deleteCommand, RelayCommand editCommand)
        {
            this.Showtime = _Showtime;
            this.DeleteCommand = _deleteCommand;
            this.EditCommand = editCommand;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class ShowtimeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<ShowtimeCommand> ShowtimesList { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        private readonly DbCinemaManagementContext _context;
        private ICollection<Ticket> TicketsList { get; set; }
        public List<Movie> MoviesList { get; set; }

        public ShowtimeCommand SelectedShowtime { get; set; }
        public RelayCommand AddNewShowtimeCommand { get; set; }

        private ShowTimeModifyWindow _showTimeModifyWindow;

        private int _totalTickets;
        public int TotalTickets
        {
            get { return _totalTickets; }
            set
            {
                _totalTickets = value;
                OnPropertyChanged(nameof(TotalTickets));
            }
        }
        private int _totalSaleTickets;
        public int TotalSaleTickets
        {
            get { return _totalSaleTickets; }
            set
            {
                _totalSaleTickets = value;
                OnPropertyChanged(nameof(TotalSaleTickets));
            }
        }

        public ShowtimeViewModel(DbCinemaManagementContext context)
        {
            _context = context;
            //MoviesList = GenerateMovieListData();
            //ShowtimesList = GenerateSampleData();
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            EditCommand = new RelayCommand(OnEdit, CanEdit);
            ShowtimesList = GenerateSampleData(DeleteCommand, EditCommand);
            if (ShowtimesList.Count > 0)
            SelectedShowtime = ShowtimesList[0];
            TotalTickets = CalculateTotalTickets();
            TotalSaleTickets = CalculateTotalSaleTickets();
            AddNewShowtimeCommand = new RelayCommand(OnShowtimeModifyWindow);
        }

        private bool CanEdit(object arg)
        {
            return SelectedShowtime != null;
        }

        private void OnEdit(object obj)
        {
            if (_showTimeModifyWindow != null)
            {
                // Delete the previous window
                _showTimeModifyWindow.Close();
                _showTimeModifyWindow = null;
            }

            _showTimeModifyWindow = new ShowTimeModifyWindow(SelectedShowtime.Showtime);
            _showTimeModifyWindow.Activate();
            _showTimeModifyWindow.Closed += _showTimeModifyWindow_Closed;
        }

        public ShowtimeViewModel()
        {
            //MoviesList = GenerateMovieListData();
            //ShowtimesList = GenerateSampleData();
            //TotalTickets = CalculateTotalTickets();
            //TotalSaleTickets = CalculateTotalSaleTickets();
        }

        private async Task DeleteShowtimeAsync(ShowTime showTime)
        {
            try
            {
                var showTimeToDelete = _context.ShowTimes.Include(st => st.Tickets).FirstOrDefault(st => st.ShowTimeId == showTime.ShowTimeId);
                // Remove the available tickets
                for (int i = 0; i < showTimeToDelete.Tickets.Count; i++)
                {
                    if (showTimeToDelete.Tickets.ElementAt(i).IsAvailable == true)
                    {
                        _context.Tickets.Remove(showTimeToDelete.Tickets.ElementAt(i));
                    }
                }
                // Remove the showtime
                _context.ShowTimes.Remove(showTimeToDelete);
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting genre: {ex.Message}");
            }
        }

        private bool CanDelete(object parameter)
        {

            return SelectedShowtime != null;
        }

        private void OnDelete(object obj)
        {
            // Print a debug message to indicate that the OnDelete method is being called
            Debug.WriteLine("OnDelete method called.");

            // Check if SelectedGenre is correctly set
            if (SelectedShowtime != null)
            {
                Debug.WriteLine($"Deleting Genre: {SelectedShowtime.Showtime.ShowTimeId}");

                DeleteShowtimeAsync(SelectedShowtime.Showtime); // Call the method to delete from the database

                ShowtimesList.Remove(SelectedShowtime);
            }
            else
            {
                // Print a debug message if SelectedGenre is null
                Debug.WriteLine("SelectedGenre is null. Cannot delete.");
            }

        }

        //private void GenerateTicketsData()
        //{
        //    if (TicketsList == null)
        //    {
        //        TicketsList = new List<Ticket>
        //        {
        //            new Ticket { TicketId = 1, IsAvailable = true ,Price = 10, ShowTimeId=1 },
        //            new Ticket { TicketId = 2, IsAvailable = false ,Price = 10, ShowTimeId=1 },
        //            new Ticket { TicketId = 3, IsAvailable = true ,Price = 10, ShowTimeId=1 },
        //            new Ticket { TicketId = 4, IsAvailable = true ,Price = 10, ShowTimeId=1 },
        //            new Ticket { TicketId = 5, IsAvailable = true ,Price = 10, ShowTimeId=1 },
        //            new Ticket { TicketId = 6, IsAvailable = true ,Price = 10, ShowTimeId=2 },
        //            new Ticket { TicketId = 7, IsAvailable = false ,Price = 10, ShowTimeId=2 },
        //            new Ticket { TicketId = 8, IsAvailable = true ,Price = 10, ShowTimeId=2 },
        //            new Ticket { TicketId = 9, IsAvailable = true ,Price = 10, ShowTimeId=3 },
        //            new Ticket { TicketId = 10, IsAvailable = true ,Price = 10, ShowTimeId=3 },
        //            new Ticket { TicketId = 11, IsAvailable = false ,Price = 10, ShowTimeId=3 },
        //            new Ticket { TicketId = 12, IsAvailable = false ,Price = 10, ShowTimeId=3 },
        //            new Ticket { TicketId = 13, IsAvailable = false ,Price = 10, ShowTimeId=3 },
        //            new Ticket { TicketId = 14, IsAvailable = true ,Price = 10, ShowTimeId=3 },
        //            new Ticket { TicketId = 15, IsAvailable = true ,Price = 10, ShowTimeId=1 },
        //            new Ticket { TicketId = 16, IsAvailable = true ,Price = 10, ShowTimeId=1 },
        //            new Ticket { TicketId = 17, IsAvailable = true ,Price = 10, ShowTimeId=1 },

        //        };
        //    }
        //}

        private ObservableCollection<ShowtimeCommand> GenerateSampleData(RelayCommand DeleteCommand, RelayCommand EditCommand)
        {
            // Generate sample data for G
            ObservableCollection<ShowtimeCommand> res = new ObservableCollection<ShowtimeCommand>();
            var ShowTimes = _context.ShowTimes
                .Include(m => m.Tickets)
                .Include(m => m.Movie).ToList();

            foreach (var show in ShowTimes)
            {
                res.Add(new ShowtimeCommand(show, DeleteCommand, EditCommand));
            }
            return res;

            //var showTime1 = new ShowTime
            //{
            //    ShowTimeId = 1,
            //    MovieId = 1,
            //    ShowDate = DateTime.Parse("2024-03-19 10:30:00"),
            //    MaxRow = 12,
            //    MaxCol = 10,
            //    Movie = GetMovieById(1),
            //    Tickets = new List<Ticket>
            //    {
            //        new Ticket { TicketId = 1, IsAvailable = true, Price = 10, ShowTimeId = 1 },
            //        new Ticket { TicketId = 2, IsAvailable = false, Price = 10, ShowTimeId = 1 },
            //        new Ticket { TicketId = 3, IsAvailable = true, Price = 10, ShowTimeId = 1 },
            //        // Add more tickets for ShowTime 1 as needed
            //    }
            //};
            //res.Add(showTime1);

            //var showTime2 = new ShowTime
            //{
            //    ShowTimeId = 2,
            //    MovieId = 2,
            //    ShowDate = DateTime.Parse("2024-02-15 10:30:00"),
            //    MaxRow = 12,
            //    MaxCol = 10,
            //    Movie = GetMovieById(2),
            //    Tickets = new List<Ticket>
            //    {
            //        new Ticket { TicketId = 4, IsAvailable = true, Price = 10, ShowTimeId = 2 },
            //        new Ticket { TicketId = 5, IsAvailable = true, Price = 10, ShowTimeId = 2 },
            //        // Add more tickets for ShowTime 2 as needed
            //    }
            //};

            //// Add ShowTime 2 to the result list
            //res.Add(showTime2);



            //var showTime3 = new ShowTime
            //{
            //    ShowTimeId = 3,
            //    MovieId = 3,
            //    ShowDate = DateTime.Parse("2024-03-20 15:30:00"),
            //    MaxRow = 12,
            //    MaxCol = 10,
            //    Movie = GetMovieById(3),
            //    Tickets = new List<Ticket>
            //    {
            //        new Ticket { TicketId = 6, IsAvailable = true, Price = 10, ShowTimeId = 3 },
            //        new Ticket { TicketId = 7, IsAvailable = false, Price = 10, ShowTimeId = 3 },
            //        new Ticket { TicketId = 8, IsAvailable = true, Price = 15, ShowTimeId = 3 },
            //        new Ticket { TicketId = 9, IsAvailable = false, Price = 20, ShowTimeId = 3 },
            //        new Ticket { TicketId = 10, IsAvailable = true, Price = 10, ShowTimeId = 3 },
            //        // Add more tickets for ShowTime 3 as needed
            //    }
            //};

            //// Add ShowTime 3 to the result list
            //res.Add(showTime3);


            //return res;
        }

        private Movie GetMovieById(int movieId)
        {
            // Retrieve a movie from the MoviesList based on its ID
            Debug.WriteLine("The movie: ", MoviesList.FirstOrDefault(movie => movie.MovieId == movieId).Title);
            return MoviesList.FirstOrDefault(movie => movie.MovieId == movieId);
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private List<Movie> GenerateMovieListData()
        {

            // Generate sample data for G
            List<Movie> res = new List<Movie>();
            res.Add(new Movie
            {
                MovieId = 1,
                Title = "Avatar: The way of water",
                Duration = 120,
                PublishYear = 2022,
                Imdbrating = 7.8,
                PosterPath = "/Assets/Images/Poster/avatar_the_way_of_water.jpg",
                Description = "\"Avatar: The Way of Water\" is a sequel to the first \"Avatar\" film, set more than a decade after the events of the first film1. The story follows the Sully family (Jake, Neytiri, and their kids) as they seek refuge with the aquatic Metkayina clan of Pandora, a habitable exomoon on which they live2",
                TrailerPath = "/Assets/Videos/avatar.mp4",

            });
            res.Add(new Movie
            {
                MovieId = 2,
                Title = "Dune Part Two",
                Duration = 120,
                PublishYear = 2022,
                Imdbrating = 7.8,
                PosterPath = "/Assets/Images/Poster/dune2.jpg",
                Description = "Dune 2 is the sequel to Dune (2021)1. It is the second of a two-part adaptation of the 1965 novel Dune by Frank Herbert1. The movie follows Paul Atreides as he unites with the Fremen people of the desert planet Arrakis to wage war against House Harkonnen1",
                TrailerPath = "/Assets/Videos/dune2.mp4",

            });
            res.Add(new Movie
            {
                MovieId = 3,
                Title = "Avatar: The way of water",
                Duration = 120,
                PublishYear = 2022,
                Imdbrating = 7.8,
                PosterPath = "/Assets/Images/Poster/avatar_the_way_of_water.jpg",
                Description = "\"Avatar: The Way of Water\" is a sequel to the first \"Avatar\" film, set more than a decade after the events of the first film1. The story follows the Sully family (Jake, Neytiri, and their kids) as they seek refuge with the aquatic Metkayina clan of Pandora, a habitable exomoon on which they live2",
                TrailerPath = "/Assets/Videos/avatar.mp4",

            });

            return res;
        }
        public int CalculateTotalTickets()
        {
            int totalTickets = 0;
            foreach (var showtime in ShowtimesList)
            {
                totalTickets = (showtime.Showtime?.MaxRow ?? 0) * (showtime.Showtime?.MaxCol ?? 0);
                Debug.WriteLine("Total: " + totalTickets.ToString());
            }
            return totalTickets;
        }
        public int CalculateTotalSaleTickets()
        {
            int totalSaleTickets = 0;
            foreach (var showTime in ShowtimesList)
            {
                totalSaleTickets = 0;
                if (showTime.Showtime.Tickets != null)
                {
                    totalSaleTickets += showTime.Showtime.Tickets.Count(ticket => !ticket?.IsAvailable ?? false);
                }
            }
            return totalSaleTickets;
        }

        public void OnShowtimeModifyWindow(object obj)
        {
            this._showTimeModifyWindow = new ShowTimeModifyWindow(null);
            _showTimeModifyWindow.Activate();
            _showTimeModifyWindow.Closed += _showTimeModifyWindow_Closed;
        }

        private void _showTimeModifyWindow_Closed(object sender, Microsoft.UI.Xaml.WindowEventArgs args)
        {
            if (_showTimeModifyWindow.returnVal.Item1 && _showTimeModifyWindow.returnVal.Item3 != -1)
            {
                // Check if the showtime is already in the list
                var currRow = ShowtimesList.Where(t => t.Showtime.ShowTimeId == _showTimeModifyWindow.returnVal.Item3).FirstOrDefault();
                var newShowtime = _context.ShowTimes
                    .Where(st => st.ShowTimeId == _showTimeModifyWindow.returnVal.Item3)
                    .Include(st => st.Movie)
                    .Include(st => st.Tickets)
                    .FirstOrDefault();
                if (currRow == null)
                {
                    ShowtimesList.Insert(0, new ShowtimeCommand(newShowtime, DeleteCommand, EditCommand));
                    TotalTickets = CalculateTotalTickets();
                    TotalSaleTickets = CalculateTotalSaleTickets();
                } else
                {
                    currRow.Showtime = newShowtime;
                    TotalTickets = CalculateTotalTickets();
                    TotalSaleTickets = CalculateTotalSaleTickets();
                }
            }
        }
    }
    public class TotalShowtimesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<ShowtimeCommand> showtimes)
            {
                int total = 0;
                total = showtimes.Count;
                return total;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ShowDateViewConverter : IValueConverter
    {
        public class CombinedItem
        {
            public String Datetime { get; set; }
            public String Icon { get; set; }
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime showDate)
            {
                var combinedItems = new List<CombinedItem>();
                string formattedDateTime = showDate.ToString("MM/dd HH:mm");
                Debug.WriteLine("DEBUG: " + formattedDateTime);
                string[] temp = formattedDateTime.Split(' ');
                Debug.WriteLine(temp[0] + " " + temp[1]);


                for (int i = 0; i < temp.Length; i++)
                {
                    combinedItems.Add(new CombinedItem
                    {
                        Datetime = temp[i],
                        Icon = (i < temp.Length - 1) ? "\uF83F" : string.Empty
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

    public class TotalTicketsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ShowtimeCommand showtime)
            {

                int totalTickets = 0;

                totalTickets = (showtime.Showtime?.MaxRow ?? 0) * (showtime.Showtime?.MaxCol ?? 0);

                return totalTickets;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TotalSaleTicketsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ShowtimeCommand showTime)
            {
                int totalSaleTickets = 0;

                if (showTime.Showtime.Tickets != null)
                {
                    totalSaleTickets = showTime.Showtime.Tickets.Count(ticket => !ticket?.IsAvailable ?? false);
                }

                return totalSaleTickets;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
