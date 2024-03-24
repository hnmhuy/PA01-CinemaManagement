using CinemaManagement.Models;
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
    public class ShowtimeModifyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        // Override OnPropertyChanged method
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private DbCinemaManagementContext db = new DbCinemaManagementContext();
        private int _mapWidth { get; set; }
        public int MapWidth
        {
            get { return _mapWidth; }
            set
            {
                _mapWidth = value;
                OnPropertyChanged(nameof(MapWidth));
            }
        }

        private int _mapHeight { get; set; }
        public int MapHeight
        {
            get { return _mapHeight; }
            set
            {
                _mapHeight = value;
                OnPropertyChanged(nameof(MapHeight));
            }
        }

        private int _numberOfRows { get; set; }
        public int NumberOfRows
        {
            get { return _numberOfRows; }
            set
            {
                _numberOfRows = value;
                OnPropertyChanged("NumberOfRows");
            }
        }

        private int _numberOfColumns { get; set; }
        public int NumberOfColumns
        {
            get { return _numberOfColumns; }
            set
            {
                _numberOfColumns = value;
                OnPropertyChanged("NumberOfColumns");
            }
        }

        public RelayCommand UpdateMapCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public ObservableCollection<Ticket> TicketList { get; set; } = new ObservableCollection<Ticket>();
        public ObservableCollection<string> rowsName { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<Movie> MovieList { get; set; } = new ObservableCollection<Movie>();
        private Movie selectedMovie { get; set; }
        public Movie SelectedMovie
        {
            get { return selectedMovie; }
            set
            {
                selectedMovie = value;
                Debug.WriteLine("Selected movie: " + selectedMovie.Title);
                OnPropertyChanged(nameof(SelectedMovie));
            }
        }
        private DateTime selectedDate { get; set; }
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                Debug.WriteLine("Selected date: " + selectedDate);
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        private TimeSpan selectedTime { get; set; }
        public TimeSpan SelectedTime
        {
            get { return selectedTime; }
            set
            {
                selectedTime = value;
                Debug.WriteLine("Selected time: " + selectedTime);
                OnPropertyChanged(nameof(SelectedTime));
            }
        }
        private double _vipPrice { get; set; }
        public double VipPrice
        {
            get { return _vipPrice; }
            set
            {
                _vipPrice = value;
                UpdateTicketPrice();
                OnPropertyChanged(nameof(VipPrice));
            }
        }

        private double _normalPrice { get; set; }
        public double NormalPrice
        {
            get { return _normalPrice; }
            set
            {
                _normalPrice = value;
                UpdateTicketPrice();
                OnPropertyChanged(nameof(NormalPrice));
            }
        }

        public ShowtimeModifyViewModel()
        {
            UpdateMapCommand = new RelayCommand(UpdateMap);
            SaveCommand = new RelayCommand(OnSave);
            LoadMovie();
            VipPrice = 0;
            NormalPrice = 0;
        }

        private void LoadMovie()
        {
            var temp = db.Movies.ToList();
            foreach (Movie m in temp)
            {
                MovieList.Add(m);
            }
        }

        public (bool, string, int) returnValue;
        private bool isSaved = false;
        public bool IsSaved
        {
            get { return isSaved; }
            set
            {
                isSaved = value;
                OnPropertyChanged(nameof(IsSaved));
            }
        }

        private string ConvertIndexToLeter(int index)
        {
            return ((char)(index + 65)).ToString();
        }

        private void ConvertIndexesToLetters(int maxRow)
        {
            rowsName.Clear();
            for (int i = 0; i < maxRow; i++)
            {
                rowsName.Add(ConvertIndexToLeter(i));
            }
        }

        public void GenerateTickets()
        {
            TicketList.Clear();
            for (int i = 0; i < NumberOfRows; i++)
            {
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    TicketList.Add(new Ticket()
                    {
                        Row = ConvertIndexToLeter(i),
                        Col = j,
                        Price = 0,
                        IsAvailable = true,
                        IsVip = false
                    });
                }
            }
        }

        public void CalculateMapSize()
        {
            MapHeight = NumberOfRows * 50 + (NumberOfRows) * 12;
            MapWidth = NumberOfColumns * 50 + (NumberOfColumns) * 12;
        }

        public bool CanUpdateMap(object obj)
        {
            return MapHeight > 0 && MapWidth > 0;
        }

        public void UpdateMap(object obj)
        {
            CalculateMapSize();
            ConvertIndexesToLetters(NumberOfRows);
            GenerateTickets();
        }

        public void UpdateTicketPrice()
        {
            foreach (Ticket t in TicketList)
            {
                if (t.IsVip)
                {
                    t.Price = VipPrice;
                }
                else
                {
                    t.Price = NormalPrice;
                }
            }
        }

        public bool CanSave()
        {
            bool res = SelectedMovie != null;
            if (!res) { returnValue = (false, "Please select a movie", -1); IsSaved = false; return false; }
            res = res && SelectedDate > DateTime.Now;
            if (!res) { returnValue = (false, "Please select a valid date", -1); IsSaved = false; return false; }
            res = res && NumberOfColumns > 0 && NumberOfRows > 0;
            if (!res) { returnValue = (false, "Please select a valid number of rows and columns", -1); IsSaved = false; return false; }
            res = res && NormalPrice > 0;
            if (!res) { returnValue = (false, "Please enter a valid price for normal ticket", -1); IsSaved = false; return false; }
            // Check if the VIP price is set and there is at least one VIP seat
            if(TicketList.Any(x => x.IsVip))
            {
                res = res && VipPrice > 0;
            }
            if (!res) { returnValue = (false, "Please enter a valid price for VIP ticket", -1); IsSaved = false; return false; }

            return res;
        }

        public void OnSave(object obj)
        {
            if (CanSave() && db.Database.CanConnect())
            {
                UpdateTicketPrice();
                var newShowtime = new ShowTime()
                {
                    MaxRow = NumberOfRows,
                    MaxCol = NumberOfColumns,
                    MovieId = SelectedMovie.MovieId,
                    ShowDate = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, SelectedTime.Hours, SelectedTime.Minutes, SelectedTime.Seconds),
                };
                db.ShowTimes.Add(newShowtime);
                // Loop through the ticket list and add them to the database
                foreach (Ticket t in TicketList)
                {
                    db.Tickets.Add(new Ticket()
                    {
                        ShowTime = newShowtime,
                        Row = t.Row,
                        Col = t.Col,
                        Price = t.Price,
                        IsAvailable = t.IsAvailable,
                        IsVip = t.IsVip
                    });
                }
                try
                {
                    db.SaveChanges();
                    returnValue = (true, "Showtime added successfully", newShowtime.ShowTimeId);
                    IsSaved = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    returnValue = (false, "Error adding showtime! " + e.Message, -1);
                    IsSaved = false;
                }
            }
        }

        public void OnCancle(object obj)
        {
            returnValue = (false, "Operation canceled", -1);
            IsSaved = false;
        }
    }
}
