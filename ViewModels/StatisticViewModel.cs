
using CinemaManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls.Primitives;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.UI;
using static CinemaManagement.ViewModels.StatisticViewModel;

namespace CinemaManagement.ViewModels
{

    class StatisticViewModel : ObservableObject, INotifyPropertyChanged
    {


        public DateTime minDate = new DateTime(2020, 1, 1);
        public DateTime maxDate = DateTime.Now;
        public ObservableCollection<ShowTime> ShowTimeList { get; set; }
        public ObservableCollection<string> ShowTimeDate { get; set; }
        public bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public ISeries[] _seriesMonth;
        public ISeries[] SeriesMonth
        {
            get { return _seriesMonth; }
            set
            {
                _seriesMonth = value;
                OnPropertyChanged(nameof(SeriesMonth));
            }
        }

        public ISeries[] _seriesYear;
        public ISeries[] SeriesYear
        {
            get { return _seriesYear; }
            set
            {
                _seriesYear = value;
                OnPropertyChanged(nameof(SeriesYear));
            }
        }

        public ISeries[] _seriesDay;
        public ISeries[] SeriesDay {
            get { return _seriesDay; }
            set
            {
                _seriesDay = value;
                OnPropertyChanged(nameof(SeriesDay));
            }
        }
        public ISeries[] _seriesWeek;
        public ISeries[] SeriesWeek
        {
            get { return _seriesWeek; }
            set
            {
                _seriesWeek = value;
                OnPropertyChanged(nameof(SeriesWeek));
            }
        }

        public Axis[] XAxesMonth { get; set; }
        public Axis[] XAxesDay { get; set; }
        public Axis[] XAxesWeek { get; set; }
        public Axis[] XAxesYear { get; set; }
        public RelayCommand CheckRevenue { get; set; }
        public RelayCommand Search { get; set; }
        public RelayCommand Clear { get; set; }
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private LabelVisual _titleMonth;
        public string _selectedShowTime;
        public string SelectedShowTime
        { 
            get { return _selectedShowTime; }
            set
            {
                _selectedShowTime = value;
                OnPropertyChanged(nameof(SelectedShowTime));
                UpdateChart();
            }
        }
        public LabelVisual TitleMonth
        {
            get { return _titleMonth; }
            set
            {
                _titleMonth = value;
                OnPropertyChanged(nameof(TitleMonth));
            }
        }

        public Movie _movieResult;

        public Movie MovieResult
        {
            get { return _movieResult; }
            set
            {
                _movieResult = value;
                OnPropertyChanged(nameof(MovieResult));
            }
        }

        private LabelVisual _titleDay;

        public LabelVisual TitleDay
        {
            get { return _titleDay; }
            set
            {
                _titleDay = value;
                OnPropertyChanged(nameof(TitleDay));
            }
        }

        private LabelVisual _titleYear;

        public LabelVisual TitleYear
        {
            get { return _titleYear; }
            set
            {
                _titleYear = value;
                OnPropertyChanged(nameof(TitleYear));
            }
        }

        private LabelVisual _titleWeek;

        public LabelVisual TitleWeek
        {
            get { return _titleWeek; }
            set
            {
                _titleWeek = value;
                OnPropertyChanged(nameof(TitleWeek));
            }
        }

        public DateTimeOffset? _selectedYear;

        public DateTimeOffset? SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
                UpdateChart();
            }
        }

        public DateTimeOffset? _selectedWeek;
        public DateTimeOffset? SelectedWeek
        {
            get { return _selectedWeek; }
            set
            {
                _selectedWeek = value;
                OnPropertyChanged(nameof(SelectedWeek));
            }
        }

        public DateTimeOffset? _SelectedDayFrom;
        public DateTimeOffset? SelectedDayFrom
        {
            get { return _SelectedDayFrom; }
            set
            {
                _SelectedDayFrom = value;
                OnPropertyChanged(nameof(SelectedDayFrom));
            }
        }

        public DateTimeOffset? _SelectedDayTo;
        public DateTimeOffset? SelectedDayTo
        {
            get { return _SelectedDayTo; }
            set
            {
                _SelectedDayTo = value;
                Debug.WriteLine(_SelectedDayTo.ToString());
                OnPropertyChanged(nameof(SelectedDayTo));
            }
        }

        public ObservableCollection<string> Modes { get; set; }

        public ObservableCollection<Movie> _allMovies;
        public ObservableCollection<string> _allMovieTitles;
        public ObservableCollection<string> _filteredMovies;
        public ObservableCollection<string> FilteredMovies
        {
            get => _filteredMovies;
            set
            {
                _filteredMovies = value;
                OnPropertyChanged(nameof(FilteredMovies));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterMovies();
            }
        }

        private Movie _selectedItem;
        public Movie SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public DbCinemaManagementContext db = new DbCinemaManagementContext();
        public StatisticViewModel()
        {
            SelectedDayFrom = new DateTimeOffset(DateTime.Now);
            SelectedDayTo = new DateTimeOffset(DateTime.Now);
            SelectedYear = new DateTimeOffset(DateTime.Now);
            SelectedWeek = new DateTimeOffset(DateTime.Now);
            var values = new ObservableCollection<DateTimePoint>();
            ShowTimeList = new ObservableCollection<ShowTime>();
            ShowTimeDate = new ObservableCollection<string>();
            ShowTimeDate.Add("All");
            var allMovies = db.Movies.ToList();

            Debug.WriteLine(allMovies.Count);

            _allMovies = new ObservableCollection<Movie>(allMovies);
            _allMovieTitles = new ObservableCollection<string>();
            foreach(Movie movie in _allMovies)
            {
                _allMovieTitles.Add(movie.Title);
            }
            _filteredMovies = new ObservableCollection<string>();
            foreach (var movie in allMovies)
            {
                _filteredMovies.Add(movie.Title);
            }

            TitleDay =
            new LabelVisual
            {
                Text = "Revenue in Days (million)",
                TextSize = 15,
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
            XAxesDay = new Axis[]
            {
                    new DateTimeAxis(TimeSpan.FromDays(1), date => "Day " + date.ToString("dd"))
            };


            TitleYear =
            new LabelVisual
            {
                Text = "Revenue in Years (million)",
                TextSize = 15,
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
            XAxesYear = new Axis[]
            {
                new DateTimeAxis(TimeSpan.FromDays(365), date => date.ToString("yyyy"))
            };

            TitleWeek =
            new LabelVisual
            {
                Text = "Revenue in a Week (million)",
                TextSize = 15,
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
            XAxesWeek = new Axis[]
            {
                    new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd", CultureInfo.InvariantCulture))
            };
            
            CalculateMovieRevenueFromDayToDay(SelectedDayFrom, SelectedDayTo);
            CalculateMovieRevenueInAWeek(SelectedWeek);
            CalculateMovieRevenueInMonths(SelectedYear);
            CalculateMovieRevenueInYears();

            //SeriesMonth = new ISeries[]
            //{
            //    new PolarLineSeries<ObservablePolarPoint>
            //    {
            //        Values = new[]
            //    {
            //        new ObservablePolarPoint(0, 10),
            //        new ObservablePolarPoint(45, 15),
            //        new ObservablePolarPoint(90, 20),
            //        new ObservablePolarPoint(135, 25),
            //        new ObservablePolarPoint(180, 30),
            //        new ObservablePolarPoint(225, 35),
            //        new ObservablePolarPoint(270, 40),
            //        new ObservablePolarPoint(315, 45),
            //        new ObservablePolarPoint(360, 50),
            //    },
            //        IsClosed = false,
            //        Fill = null
            //    }
            //};


            MovieResult = SelectedItem;
            Search = new RelayCommand(SearchItem);
            Clear = new RelayCommand(ClearItem);
        }

        public class MonthlyRevenue
        {
            public DateTime Month { get; set; }
            public double Revenue { get; set; }
        }

        public class DailyRevenue
        {
            public DateTime Date { get; set; }
            public double Revenue { get; set; }
        }

        public void CalculateShowTimeRevenueInYears(ShowTime ShowTimeDate)
        {
            Debug.WriteLine("Function runs.");
            var yearlyRevenues = new ObservableCollection<DailyRevenue>();
            DateTimeOffset startDate = new DateTimeOffset(SelectedDayFrom.Value.Year, SelectedDayFrom.Value.Month, SelectedDayFrom.Value.Day, 0, 0, 0, SelectedWeek.Value.Offset);
            Debug.WriteLine(SelectedDayFrom);
            Debug.WriteLine(SelectedDayTo);

            int YearNow = DateTime.Now.Year;

            for (int year = 2020; year <= YearNow; year++)
            {
                var yearlyRevenue = db.ShowTimes
                    .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                    .SelectMany(st => st.Tickets)
                    .Where(t => !t.IsAvailable &&
                                t.Bill.BookingTime.Year == year && t.ShowTime.ShowTimeId == ShowTimeDate.ShowTimeId)
                    .Sum(t => t.Price);

                yearlyRevenues.Add(new DailyRevenue
                {
                    Date = new DateTime(year, 1, 1),
                    Revenue = yearlyRevenue
                });
            }
            var values = new List<DateTimePoint>();
            foreach (var revenue in yearlyRevenues)
            {
                values.Add(new DateTimePoint(revenue.Date, revenue.Revenue));
            }
            SeriesYear = new ISeries[]
            {
                        new LineSeries<DateTimePoint> { Values = values }
            };
        }

        public void CalculateMovieRevenueInYears()
        {
            Debug.WriteLine("Function runs.");
            var yearlyRevenues = new ObservableCollection<DailyRevenue>();
            DateTimeOffset startDate = new DateTimeOffset(SelectedDayFrom.Value.Year, SelectedDayFrom.Value.Month, SelectedDayFrom.Value.Day, 0, 0, 0, SelectedWeek.Value.Offset);
            Debug.WriteLine(SelectedDayFrom);
            Debug.WriteLine(SelectedDayTo);

            int YearNow = DateTime.Now.Year;

            for (int year = 2020; year <= YearNow; year++)
            {
                var yearlyRevenue = db.ShowTimes
                    .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                    .SelectMany(st => st.Tickets)   
                    .Where(t => !t.IsAvailable &&
                                t.Bill.BookingTime.Year == year)
                    .Sum(t => t.Price);

                yearlyRevenues.Add(new DailyRevenue
                {
                    Date = new DateTime(year, 1, 1),
                    Revenue = yearlyRevenue
                });
            }
            var values = new List<DateTimePoint>();
            foreach (var revenue in yearlyRevenues)
            {
                values.Add(new DateTimePoint(revenue.Date, revenue.Revenue));
            }
            SeriesYear = new ISeries[]
            {
                        new LineSeries<DateTimePoint> { Values = values }
            };
        }

        public void CalculateMovieRevenueFromDayToDay(DateTimeOffset? SelectedDayFrom, DateTimeOffset? SelectedDayTo)
        {
            if (SelectedDayFrom <= SelectedDayTo)
            {

                if (MovieResult != null)
                {
                    Debug.WriteLine("Function runs.");
                    var dailyRevenues = new ObservableCollection<DailyRevenue>();
                    DateTimeOffset startDate = new DateTimeOffset(SelectedDayFrom.Value.Year, SelectedDayFrom.Value.Month, SelectedDayFrom.Value.Day - 1, 0, 0, 0, SelectedWeek.Value.Offset);
                    Debug.WriteLine(SelectedDayFrom);
                    Debug.WriteLine(SelectedDayTo);
                    double days = (int) (SelectedDayTo.Value - SelectedDayFrom.Value).TotalDays + 1;
                    if (days == 1)
                    {
                        for (int i = 1; i <= days; i++)
                        {
                            var monthlyRevenue = db.ShowTimes
                                .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                                .SelectMany(st => st.Tickets)
                                .Where(t => !t.IsAvailable && t.ShowTime.MovieId == MovieResult.MovieId && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                                .Sum(t => t.Price);

                            dailyRevenues.Add(new DailyRevenue
                            {
                                Date = startDate.AddDays(i).DateTime,
                                Revenue = monthlyRevenue
                            });
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= days + 1; i++)
                        {
                            var monthlyRevenue = db.ShowTimes
                                .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                                .SelectMany(st => st.Tickets)
                                .Where(t => !t.IsAvailable && t.ShowTime.MovieId == MovieResult.MovieId && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                                .Sum(t => t.Price);

                            dailyRevenues.Add(new DailyRevenue
                            {
                                Date = startDate.AddDays(i).DateTime,
                                Revenue = monthlyRevenue
                            });
                        }
                    }

                    var values = new List<DateTimePoint>();
                    foreach (var revenue in dailyRevenues)
                    {
                        values.Add(new DateTimePoint(revenue.Date, revenue.Revenue));
                    }
                    SeriesDay = new ISeries[]
                    {
                        new LineSeries<DateTimePoint> { Values = values }
                    };

                }
                else
                {
                    Debug.WriteLine("Function runs.");
                    var dailyRevenues = new ObservableCollection<DailyRevenue>();
                    DateTimeOffset startDate = new DateTimeOffset(SelectedDayFrom.Value.Year, SelectedDayFrom.Value.Month, SelectedDayFrom.Value.Day - 1, 0, 0, 0, SelectedWeek.Value.Offset);
                    Debug.WriteLine(SelectedDayFrom);
                    Debug.WriteLine(SelectedDayTo);
                    double days = (int)(SelectedDayTo.Value - SelectedDayFrom.Value).TotalDays + 1;
                    if (days == 1)
                    {
                        for (int i = 1; i <= days; i++)
                        {
                            var monthlyRevenue = db.ShowTimes
                                .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                                .SelectMany(st => st.Tickets)
                                .Where(t => !t.IsAvailable && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                                .Sum(t => t.Price);

                            dailyRevenues.Add(new DailyRevenue
                            {
                                Date = startDate.AddDays(i).DateTime,
                                Revenue = monthlyRevenue
                            });
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= days + 1; i++)
                        {
                            var monthlyRevenue = db.ShowTimes
                                .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                                .SelectMany(st => st.Tickets)
                                .Where(t => !t.IsAvailable && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                                .Sum(t => t.Price);

                            dailyRevenues.Add(new DailyRevenue
                            {
                                Date = startDate.AddDays(i).DateTime,
                                Revenue = monthlyRevenue
                            });
                        }
                    }

                    var values = new List<DateTimePoint>();
                    foreach (var revenue in dailyRevenues)
                    {
                        values.Add(new DateTimePoint(revenue.Date, revenue.Revenue));
                    }
                    SeriesDay = new ISeries[]
                    {
                        new LineSeries<DateTimePoint> { Values = values }
                    };
                }
            }
        }
        public void CalculateShowTimeRevenueFromDayToDay(DateTimeOffset? SelectedDayFrom, DateTimeOffset? SelectedDayTo, ShowTime ShowTimeDate)
        {
            if (SelectedDayFrom <= SelectedDayTo)
            {

                if (MovieResult != null)
                {
                    Debug.WriteLine("Function runs.");
                    var dailyRevenues = new ObservableCollection<DailyRevenue>();
                    DateTimeOffset startDate = new DateTimeOffset(SelectedDayFrom.Value.Year, SelectedDayFrom.Value.Month, SelectedDayFrom.Value.Day - 1, 0, 0, 0, SelectedWeek.Value.Offset);
                    Debug.WriteLine(SelectedDayFrom);
                    Debug.WriteLine(SelectedDayTo);
                    double days = (int)(SelectedDayTo.Value - SelectedDayFrom.Value).TotalDays + 1;
                    if (days == 1)
                    {
                        for (int i = 1; i <= days; i++)
                        {
                            var monthlyRevenue = db.ShowTimes
                                .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                                .SelectMany(st => st.Tickets)
                                .Where(t => !t.IsAvailable && t.ShowTime.ShowTimeId == ShowTimeDate.ShowTimeId && t.ShowTime.MovieId == MovieResult.MovieId && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                                .Sum(t => t.Price);

                            dailyRevenues.Add(new DailyRevenue
                            {
                                Date = startDate.AddDays(i).DateTime,
                                Revenue = monthlyRevenue
                            });
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= days + 1; i++)
                        {
                            var monthlyRevenue = db.ShowTimes
                                .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                                .SelectMany(st => st.Tickets)
                                .Where(t => !t.IsAvailable && t.ShowTime.ShowTimeId == ShowTimeDate.ShowTimeId && t.ShowTime.MovieId == MovieResult.MovieId && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                                .Sum(t => t.Price);

                            dailyRevenues.Add(new DailyRevenue
                            {
                                Date = startDate.AddDays(i).DateTime,
                                Revenue = monthlyRevenue
                            });
                        }
                    }

                    var values = new List<DateTimePoint>();
                    foreach (var revenue in dailyRevenues)
                    {
                        values.Add(new DateTimePoint(revenue.Date, revenue.Revenue));
                    }
                    SeriesDay = new ISeries[]
                    {
                        new LineSeries<DateTimePoint> { Values = values }
                    };

                }
            }
        }

        public void CalculateShowTimeRevenueInAWeek(DateTimeOffset? SelectedWeek, ShowTime ShowDateTime)
        {
            if (MovieResult != null)
            {
                int daysUntilMonday = (int)SelectedWeek.Value.DayOfWeek - (int)DayOfWeek.Monday;
                if (daysUntilMonday < 0)
                {
                    daysUntilMonday += 7;
                }

                DateTimeOffset startDate = new DateTimeOffset(SelectedWeek.Value.Year, SelectedWeek.Value.Month, SelectedWeek.Value.Day - daysUntilMonday - 1, 0, 0, 0, SelectedWeek.Value.Offset);

                Debug.WriteLine(startDate);
                var weeklyRevenues = new ObservableCollection<DailyRevenue>();

                for (int i = 1; i <= 7; i++)
                {

                    var monthlyRevenue = db.ShowTimes
                        .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                        .SelectMany(st => st.Tickets)
                        .Where(t => !t.IsAvailable && t.ShowTime.ShowTimeId == ShowDateTime.ShowTimeId && t.ShowTime.MovieId == MovieResult.MovieId && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                        .Sum(t => t.Price);


                    weeklyRevenues.Add(new DailyRevenue
                    {
                        Date = startDate.AddDays(i).DateTime,
                        Revenue = monthlyRevenue
                    });
                }

                var values = new List<DateTimePoint>();

                foreach (var revenue in weeklyRevenues)
                {
                    values.Add(new DateTimePoint(revenue.Date, revenue.Revenue));
                }

                SeriesWeek = new ISeries[]
                {
                new LineSeries<DateTimePoint> { Values = values }
                };

            }
            else
            {
                int daysUntilMonday = (int)SelectedWeek.Value.DayOfWeek - (int)DayOfWeek.Monday;
                if (daysUntilMonday < 0)
                {
                    daysUntilMonday += 7;
                }

                DateTimeOffset startDate = new DateTimeOffset(SelectedWeek.Value.Year, SelectedWeek.Value.Month, SelectedWeek.Value.Day - daysUntilMonday - 1, 0, 0, 0, SelectedWeek.Value.Offset);

                Debug.WriteLine(startDate);
                var weeklyRevenues = new ObservableCollection<DailyRevenue>();

                for (int i = 1; i <= 7; i++)
                {

                    var monthlyRevenue = db.ShowTimes
                        .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                        .SelectMany(st => st.Tickets)
                        .Where(t => !t.IsAvailable && t.ShowTime.ShowTimeId == ShowDateTime.ShowTimeId && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                        .Sum(t => t.Price);


                    weeklyRevenues.Add(new DailyRevenue
                    {
                        Date = startDate.AddDays(i).DateTime,
                        Revenue = monthlyRevenue
                    });
                }

                var values = new List<DateTimePoint>();

                foreach (var revenue in weeklyRevenues)
                {
                    values.Add(new DateTimePoint(revenue.Date, revenue.Revenue));
                }

                SeriesWeek = new ISeries[]
                {
                new LineSeries<DateTimePoint> { Values = values }
                };

            }
        }


        public void CalculateMovieRevenueInAWeek(DateTimeOffset? SelectedWeek)
        {
            if(MovieResult != null)
            {
                int daysUntilMonday = (int)SelectedWeek.Value.DayOfWeek - (int)DayOfWeek.Monday;
                if (daysUntilMonday < 0)
                {
                    daysUntilMonday += 7;
                }

                DateTimeOffset startDate = new DateTimeOffset(SelectedWeek.Value.Year, SelectedWeek.Value.Month, SelectedWeek.Value.Day - daysUntilMonday - 1, 0, 0, 0, SelectedWeek.Value.Offset);

                Debug.WriteLine(startDate);
                var weeklyRevenues = new ObservableCollection<DailyRevenue>();

                for (int i = 1; i <= 7; i++)
                {

                    var monthlyRevenue = db.ShowTimes
                        .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                        .SelectMany(st => st.Tickets)
                        .Where(t => !t.IsAvailable && t.ShowTime.MovieId == MovieResult.MovieId && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                        .Sum(t => t.Price);


                    weeklyRevenues.Add(new DailyRevenue
                    {
                        Date = startDate.AddDays(i).DateTime,
                        Revenue = monthlyRevenue
                    });
                }

                var values = new List<DateTimePoint>();

                foreach (var revenue in weeklyRevenues)
                {
                    values.Add(new DateTimePoint(revenue.Date, revenue.Revenue));
                }

                SeriesWeek = new ISeries[]
                {
                new LineSeries<DateTimePoint> { Values = values }
                };

            }
            else
            {
                int daysUntilMonday = (int)SelectedWeek.Value.DayOfWeek - (int)DayOfWeek.Monday;
                if (daysUntilMonday < 0)
                {
                    daysUntilMonday += 7;
                }

                DateTimeOffset startDate = new DateTimeOffset(SelectedWeek.Value.Year, SelectedWeek.Value.Month, SelectedWeek.Value.Day - daysUntilMonday - 1, 0, 0, 0, SelectedWeek.Value.Offset);

                Debug.WriteLine(startDate);
                var weeklyRevenues = new ObservableCollection<DailyRevenue>();

                for (int i = 1; i <= 7; i++)
                {

                    var monthlyRevenue = db.ShowTimes
                        .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                        .SelectMany(st => st.Tickets)
                        .Where(t => !t.IsAvailable && t.Bill.BookingTime < startDate.AddDays(i + 1) && t.Bill.BookingTime >= startDate.AddDays(i))
                        .Sum(t => t.Price);


                    weeklyRevenues.Add(new DailyRevenue
                    {
                        Date = startDate.AddDays(i).DateTime,
                        Revenue = monthlyRevenue
                    });
                }

                var values = new List<DateTimePoint>();

                foreach (var revenue in weeklyRevenues)
                {
                    values.Add(new DateTimePoint(revenue.Date, revenue.Revenue));
                }

                SeriesWeek = new ISeries[]
                {
                new LineSeries<DateTimePoint> { Values = values }
                };

            }
        }

        public void CalculateShowTimeRevenueInMonths(DateTimeOffset? SelectedYear, ShowTime ShowTimeDate)
        {
            if (MovieResult != null)
            {
                var monthlyRevenues = new List<MonthlyRevenue>();
                Debug.WriteLine(SelectedYear);
                int selectedYear = SelectedYear.Value.Year;

                for (int month = 1; month <= 12; month++)
                {
                    DateTime startOfMonth = new DateTime(selectedYear, month, 1);
                    DateTime endOfMonth = startOfMonth.AddMonths(1);

                    var monthlyRevenue = db.ShowTimes
                        .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                        .SelectMany(st => st.Tickets)
                        .Where(t => !t.IsAvailable && t.ShowTime.ShowTimeId == ShowTimeDate.ShowTimeId && t.ShowTime.MovieId == MovieResult.MovieId && t.Bill.BookingTime < endOfMonth && t.Bill.BookingTime >= startOfMonth)
                        .Sum(t => t.Price);

                    monthlyRevenues.Add(new MonthlyRevenue
                    {
                        Month = startOfMonth,
                        Revenue = monthlyRevenue
                    });
                }

                SeriesMonth = new ISeries[]
                {
                    new PolarLineSeries<ObservablePolarPoint>
                    {
                        Values = new[]
                        {
                            new ObservablePolarPoint(0, monthlyRevenues[0].Revenue),
                            new ObservablePolarPoint(1, monthlyRevenues[1].Revenue),
                            new ObservablePolarPoint(2, monthlyRevenues[2].Revenue),
                            new ObservablePolarPoint(3, monthlyRevenues[3].Revenue),
                            new ObservablePolarPoint(4, monthlyRevenues[4].Revenue),
                            new ObservablePolarPoint(5, monthlyRevenues[5].Revenue),
                            new ObservablePolarPoint(6, monthlyRevenues[6].Revenue),
                            new ObservablePolarPoint(7, monthlyRevenues[7].Revenue),
                            new ObservablePolarPoint(8, monthlyRevenues[8].Revenue),
                            new ObservablePolarPoint(9, monthlyRevenues[9].Revenue),
                            new ObservablePolarPoint(10, monthlyRevenues[10].Revenue),
                            new ObservablePolarPoint(11, monthlyRevenues[11].Revenue),
                        },
                        IsClosed = false,
                        Fill = null
                    }
                };

            }
            else
            {
                var monthlyRevenues = new List<MonthlyRevenue>();
                Debug.WriteLine(SelectedYear);
                int selectedYear = SelectedYear.Value.Year;

                for (int month = 1; month <= 12; month++)
                {
                    DateTime startOfMonth = new DateTime(selectedYear, month, 1);
                    DateTime endOfMonth = startOfMonth.AddMonths(1);

                    var monthlyRevenue = db.ShowTimes
                        .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                        .SelectMany(st => st.Tickets)
                        .Where(t => !t.IsAvailable  && t.ShowTime.ShowTimeId == ShowTimeDate.ShowTimeId && t.Bill.BookingTime < endOfMonth && t.Bill.BookingTime >= startOfMonth)
                        .Sum(t => t.Price);

                    monthlyRevenues.Add(new MonthlyRevenue
                    {
                        Month = startOfMonth,
                        Revenue = monthlyRevenue
                    });
                }

                SeriesMonth = new ISeries[]
                {
                    new PolarLineSeries<ObservablePolarPoint>
                    {
                        Values = new[]
                        {
                            new ObservablePolarPoint(0, monthlyRevenues[0].Revenue),
                            new ObservablePolarPoint(1, monthlyRevenues[1].Revenue),
                            new ObservablePolarPoint(2, monthlyRevenues[2].Revenue),
                            new ObservablePolarPoint(3, monthlyRevenues[3].Revenue),
                            new ObservablePolarPoint(4, monthlyRevenues[4].Revenue),
                            new ObservablePolarPoint(5, monthlyRevenues[5].Revenue),
                            new ObservablePolarPoint(6, monthlyRevenues[6].Revenue),
                            new ObservablePolarPoint(7, monthlyRevenues[7].Revenue),
                            new ObservablePolarPoint(8, monthlyRevenues[8].Revenue),
                            new ObservablePolarPoint(9, monthlyRevenues[9].Revenue),
                            new ObservablePolarPoint(10, monthlyRevenues[10].Revenue),
                            new ObservablePolarPoint(11, monthlyRevenues[11].Revenue),
                        },
                        IsClosed = false,
                        Fill = null
                    }
                };

            }

        }

        public void CalculateMovieRevenueInMonths(DateTimeOffset? SelectedYear)
        {
            if(MovieResult != null)
            {
                var monthlyRevenues = new List<MonthlyRevenue>();
                Debug.WriteLine(SelectedYear);
                int selectedYear = SelectedYear.Value.Year;

                for (int month = 1; month <= 12; month++)
                {
                    DateTime startOfMonth = new DateTime(selectedYear, month, 1);
                    DateTime endOfMonth = startOfMonth.AddMonths(1);

                    var monthlyRevenue = db.ShowTimes
                        .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                        .SelectMany(st => st.Tickets)
                        .Where(t => !t.IsAvailable && t.ShowTime.MovieId == MovieResult.MovieId && t.Bill.BookingTime < endOfMonth && t.Bill.BookingTime >= startOfMonth)
                        .Sum(t => t.Price);

                    monthlyRevenues.Add(new MonthlyRevenue
                    {
                        Month = startOfMonth,
                        Revenue = monthlyRevenue
                    });
                }

                SeriesMonth = new ISeries[]
                {
                    new PolarLineSeries<ObservablePolarPoint>
                    {
                        Values = new[]
                        {
                            new ObservablePolarPoint(0, monthlyRevenues[0].Revenue),
                            new ObservablePolarPoint(1, monthlyRevenues[1].Revenue),
                            new ObservablePolarPoint(2, monthlyRevenues[2].Revenue),
                            new ObservablePolarPoint(3, monthlyRevenues[3].Revenue),
                            new ObservablePolarPoint(4, monthlyRevenues[4].Revenue),
                            new ObservablePolarPoint(5, monthlyRevenues[5].Revenue),
                            new ObservablePolarPoint(6, monthlyRevenues[6].Revenue),
                            new ObservablePolarPoint(7, monthlyRevenues[7].Revenue),
                            new ObservablePolarPoint(8, monthlyRevenues[8].Revenue),
                            new ObservablePolarPoint(9, monthlyRevenues[9].Revenue),
                            new ObservablePolarPoint(10, monthlyRevenues[10].Revenue),
                            new ObservablePolarPoint(11, monthlyRevenues[11].Revenue),
                        },
                        IsClosed = false,
                        Fill = null
                    }
                };

            } else
            {
                var monthlyRevenues = new List<MonthlyRevenue>();
                Debug.WriteLine(SelectedYear);
                int selectedYear = SelectedYear.Value.Year;

                for (int month = 1; month <= 12; month++)
                {
                    DateTime startOfMonth = new DateTime(selectedYear, month, 1);
                    DateTime endOfMonth = startOfMonth.AddMonths(1);

                    var monthlyRevenue = db.ShowTimes
                        .Include(st => st.Tickets).ThenInclude(t => t.Bill)
                        .SelectMany(st => st.Tickets)
                        .Where(t => !t.IsAvailable && t.Bill.BookingTime < endOfMonth && t.Bill.BookingTime >= startOfMonth)
                        .Sum(t => t.Price);

                    monthlyRevenues.Add(new MonthlyRevenue
                    {
                        Month = startOfMonth,
                        Revenue = monthlyRevenue
                    });
                }

                SeriesMonth = new ISeries[]
                {
                    new PolarLineSeries<ObservablePolarPoint>
                    {
                        Values = new[]
                        {
                            new ObservablePolarPoint(0, monthlyRevenues[0].Revenue),
                            new ObservablePolarPoint(1, monthlyRevenues[1].Revenue),
                            new ObservablePolarPoint(2, monthlyRevenues[2].Revenue),
                            new ObservablePolarPoint(3, monthlyRevenues[3].Revenue),
                            new ObservablePolarPoint(4, monthlyRevenues[4].Revenue),
                            new ObservablePolarPoint(5, monthlyRevenues[5].Revenue),
                            new ObservablePolarPoint(6, monthlyRevenues[6].Revenue),
                            new ObservablePolarPoint(7, monthlyRevenues[7].Revenue),
                            new ObservablePolarPoint(8, monthlyRevenues[8].Revenue),
                            new ObservablePolarPoint(9, monthlyRevenues[9].Revenue),
                            new ObservablePolarPoint(10, monthlyRevenues[10].Revenue),
                            new ObservablePolarPoint(11, monthlyRevenues[11].Revenue),
                        },
                        IsClosed = false,
                        Fill = null
                    }
                };

            }

        }

        void validate()
        {
            CalculateMovieRevenueFromDayToDay(SelectedDayFrom, SelectedDayTo);
            CalculateMovieRevenueInAWeek(SelectedWeek);
            CalculateMovieRevenueInMonths(SelectedYear);
        }

        private void FilterMovies()
        {
            var splitText = SearchText?.ToLower().Split(" ") ?? Array.Empty<string>();

            if (splitText.Length == 0)
            {
                FilteredMovies = new ObservableCollection<string>(_allMovieTitles);
                return;
            }

            var filteredItems = _allMovieTitles.Where(cat =>
            {
                return splitText.All(key => cat.ToLower().Contains(key));
            });

            FilteredMovies.Clear();
            foreach (var item in filteredItems)
            {
                FilteredMovies.Add(item);
            }

            if (FilteredMovies.Count == 0)
            {
                FilteredMovies.Add("No results found");
            }
        }


        private void SearchItem(object obj)
        {
            foreach(var item in _allMovies)
            {
                if (SearchText == item.Title)
                    SelectedItem = item;
            }
            MovieResult = SelectedItem;
            if(MovieResult != null)
            {
                var res = db.ShowTimes.Where(st => st.MovieId == MovieResult.MovieId).ToList();
                foreach (var item in res)
                {
                    ShowTimeList.Add(item);
                    ShowTimeDate.Add(item.ShowDate.ToString());
                }
            }
            validate();
        }

        private void ClearItem(object obj)
        {
            if(MovieResult != null)
            {
                if(ShowTimeList != null)
                {
                    ShowTimeList.Clear();
                    MovieResult = null;
                }
                MovieResult = null;
                SelectedItem = null;
            }
        }



        public PolarAxis[] MonthAxes { get; set; } =
        {
            new PolarAxis
            {
                MinLimit = 0,
                MaxLimit = 12,
                Labeler = angle => {
                    int monthIndex = (int)Math.Round(angle) + 1;
                    monthIndex = monthIndex == 13 ? 1 : monthIndex;
                    string monthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(monthIndex);
                    return monthName;
                },
                ForceStepToMin = true,
                MinStep = 1
            }
        };
        private void UpdateChart()
        {
            if(SelectedShowTime == "All")
            {
                validate();
            } else
            {
                if (ShowTimeList != null)
                {
                    ShowTime res = new ShowTime();
                    foreach (var showtime in ShowTimeList)
                    {
                        if (showtime.ShowDate.ToString() == SelectedShowTime)
                        {
                            res = showtime;
                        }
                    }

                    CalculateShowTimeRevenueFromDayToDay(SelectedDayFrom, SelectedDayTo, res);
                    CalculateShowTimeRevenueInAWeek(SelectedWeek, res);
                    CalculateShowTimeRevenueInMonths(SelectedYear, res);
                }
            }
        }
    }
}