
using CinemaManagement.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
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

namespace CinemaManagement.ViewModels
{
    class StatisticViewModel : ObservableObject, INotifyPropertyChanged
    {
        public ObservableCollection<ShowTime> ShowTimeList { get; set; }
        public ISeries[] SeriesMonth { get; set; }
        public ISeries[] SeriesYear { get; set; }
        public ISeries[] SeriesDay { get; set; }
        public ISeries[] SeriesWeek { get; set; }
        public Axis[] XAxesMonth { get; set; }
        public Axis[] XAxesDay { get; set; }
        public Axis[] XAxesWeek { get; set; }
        public Axis[] XAxesYear { get; set; }
        public RelayCommand CheckRevenue { get; set; }
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private LabelVisual _titleMonth;

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

        private string _selectedMode;
        public string SelectedMode
        {
            get { return _selectedMode; }
            set
            {
                _selectedMode = value;
                OnPropertyChanged(nameof(SelectedMode));
            }
        }
        public ObservableCollection<string> Modes { get; set; }
        private ObservableCollection<string> _allCats;
        private ObservableCollection<string> _filteredCats;
        public ObservableCollection<string> FilteredCats
        {
            get => _filteredCats;
            set
            {
                _filteredCats = value;
                OnPropertyChanged();
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
                FilterCats();
            }
        }

        private string _selectedItem;
        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }
        public StatisticViewModel()
        {
            var values = new ObservableCollection<DateTimePoint>();
            _allCats = new ObservableCollection<string>()
            {
                "Abyssinian",
                "Aegean",
                "American Bobtail",
            };
            _filteredCats = new ObservableCollection<string>(_allCats);
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
                    new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("ddd", CultureInfo.InvariantCulture))
            };
            ObservableCollection<ShowTime> res = new ObservableCollection<ShowTime>();
            var showTime1 = new ShowTime
            {
                ShowTimeId = 1,
                MovieId = 1,
                ShowDate = DateTime.Parse("2024-03-19 10:30:00"),
                MaxRow = 12,
                MaxCol = 10,
                Tickets = new List<Ticket>
                {
                    new Ticket { TicketId = 1, IsAvailable = true, Price = 10, ShowTimeId = 1 },
                    new Ticket { TicketId = 2, IsAvailable = false, Price = 10, ShowTimeId = 1 },
                    new Ticket { TicketId = 3, IsAvailable = true, Price = 10, ShowTimeId = 1 },
                    // Add more tickets for ShowTime 1 as needed
                }
            };
            res.Add(showTime1);

            var showTime2 = new ShowTime
            {
                ShowTimeId = 2,
                MovieId = 2,
                ShowDate = DateTime.Parse("2024-02-15 10:30:00"),
                MaxRow = 12,
                MaxCol = 10,
                Tickets = new List<Ticket>
                {
                    new Ticket { TicketId = 4, IsAvailable = true, Price = 10, ShowTimeId = 2 },
                    new Ticket { TicketId = 5, IsAvailable = true, Price = 10, ShowTimeId = 2 },
                    // Add more tickets for ShowTime 2 as needed
                }
            };

            // Add ShowTime 2 to the result list
            res.Add(showTime2);



            var showTime3 = new ShowTime
            {
                ShowTimeId = 3,
                MovieId = 3,
                ShowDate = DateTime.Parse("2024-03-20 15:30:00"),
                MaxRow = 12,
                MaxCol = 10,
                Tickets = new List<Ticket>
                {
                    new Ticket { TicketId = 6, IsAvailable = true, Price = 10, ShowTimeId = 3 },
                    new Ticket { TicketId = 7, IsAvailable = false, Price = 10, ShowTimeId = 3 },
                    new Ticket { TicketId = 8, IsAvailable = true, Price = 15, ShowTimeId = 3 },
                    new Ticket { TicketId = 9, IsAvailable = false, Price = 20, ShowTimeId = 3 },
                    new Ticket { TicketId = 10, IsAvailable = true, Price = 10, ShowTimeId = 3 },
                    // Add more tickets for ShowTime 3 as needed
                }
            };

            // Add ShowTime 3 to the result list
            res.Add(showTime3);

            ShowTimeList = res;
            //DateTime selectedDate = new DateTime(2021, 1, 1);
            //int daysUntilMonday = (int)selectedDate.DayOfWeek - (int)DayOfWeek.Monday;
            //if (daysUntilMonday < 0)
            //{
            //    daysUntilMonday += 7;
            //}

            //DateTime startDate = selectedDate.AddDays(-daysUntilMonday);

            //for (int i = 0; i < 7; i++)
            //{
            //    DateTime date = startDate.AddDays(i);
            //    int value = i % 2 == 0 ? 3 : 5;
            //    values.Add(new DateTimePoint(date, value));
            //}

            for (int i = 1; i <= 7; i++)
            {
                int value = i % 2 == 0 ? 3 : 5;
                values.Add(new DateTimePoint(new DateTime(2024, 1, i), value));
            }

            SeriesDay = new ISeries[]
{
                new LineSeries<DateTimePoint> { Values = values }
};

            SeriesWeek = new ISeries[]
            {
                new LineSeries<DateTimePoint> { Values = values }
            };


            for (int i = 1; i <= 7; i++)
            {
                int value = i % 2 == 0 ? 3 : 5;
                values.Add(new DateTimePoint(new DateTime(2024 + i, 1, 1), value));
            }

            SeriesYear = new ISeries[]
            {
                new LineSeries<DateTimePoint> { Values = values }
            };
            MovieResult = new Movie
            {
                MovieId = 1,
                Title = "Avatar: The way of water",
                Duration = 120,
                PublishYear = 2022,
                Imdbrating = 7.8,
                PosterPath = "/Assets/Images/Poster/avatar_the_way_of_water.jpg",
                Description = "\"Avatar: The Way of Water\" is a sequel to the first \"Avatar\" film, set more than a decade after the events of the first film1. The story follows the Sully family (Jake, Neytiri, and their kids) as they seek refuge with the aquatic Metkayina clan of Pandora, a habitable exomoon on which they live2",
                TrailerPath = "/Assets/Videos/avatar.mp4",

            };
            CheckRevenue = new RelayCommand(UpdateChartTitle);
        }
        private void FilterCats()
        {
            var splitText = SearchText?.ToLower().Split(" ") ?? Array.Empty<string>();

            if (splitText.Length == 0)
            {
                // If search text is empty, reset FilteredCats to contain all categories
                FilteredCats = new ObservableCollection<string>(_allCats);
                return;
            }

            // Filter the cats based on the search text
            var filteredItems = _allCats.Where(cat =>
            {
                return splitText.All(key => cat.ToLower().Contains(key));
            });

            // Update the FilteredCats collection
            FilteredCats.Clear();
            foreach (var item in filteredItems)
            {
                FilteredCats.Add(item);
            }

            if (FilteredCats.Count == 0)
            {
                FilteredCats.Add("No results found");
            }
        }

        // Your existing methods and constructor

        private void OnSuggestionChosen(string selectedItem)
        {
            SelectedItem = selectedItem;
        }

        public ISeries[] MonthSeries { get; set; } =
        {
            new PolarLineSeries<ObservablePolarPoint>
            {
                Values = new[]
                {
                    new ObservablePolarPoint(0, 20),
                    new ObservablePolarPoint(1, 10),
                    new ObservablePolarPoint(2, 15),
                    new ObservablePolarPoint(3, 20),
                    new ObservablePolarPoint(4, 30),
                    new ObservablePolarPoint(5, 35),
                    new ObservablePolarPoint(6, 40),
                    new ObservablePolarPoint(7, 45),
                    new ObservablePolarPoint(8, 11),
                    new ObservablePolarPoint(9, 30),
                    new ObservablePolarPoint(10, 45),
                    new ObservablePolarPoint(11, 50),
                },
                IsClosed = false,
                Fill = null
            }
        };

        public PolarAxis[] MonthAxes { get; set; } =
        {
            new PolarAxis
            {
                // force the axis to always show 360 degrees.
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
        private void UpdateChartTitle(object obj)
        {
            
        }

    }
}