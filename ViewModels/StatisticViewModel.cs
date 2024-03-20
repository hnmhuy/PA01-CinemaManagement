using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace CinemaManagement.ViewModels
{
    class StatisticViewModel : ObservableObject
    {
        public ObservableCollection<string> Modes { get; set; }
        public string SelectedMode { get; set; }
        public LabelVisual Title { get; set; } 
        public StatisticViewModel()
        {
            var values = new ObservableCollection<DateTimePoint>();

            Modes = new ObservableCollection<string>
            {
                "Day to day",
                "Week",
                "Month",
                "Year",
            };

            Title  =
            new LabelVisual
            {
                Text = "Tổng doanh thu (million)",
                TextSize = 15,
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        DateTime selectedDate = new DateTime(2021, 1, 1);
            int daysUntilMonday = (int)selectedDate.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysUntilMonday < 0)
            {
                daysUntilMonday += 7;
            }

            DateTime startDate = selectedDate.AddDays(-daysUntilMonday);

            for (int i = 0; i < 7; i++)
            {
                DateTime date = startDate.AddDays(i);
                int value = i % 2 == 0 ? 3 : 5;
                values.Add(new DateTimePoint(date, value));
            }

            Series = new ISeries[]
            {
                new LineSeries<DateTimePoint> { Values = values }
            };

            XAxes = new Axis[]
            {
                //Xem Theo Năm
                //new DateTimeAxis(TimeSpan.FromDays(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), date => date.ToString("MMMM"))
                //Xem theo Tuần
                new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("dd", CultureInfo.InvariantCulture))
                //Xem từ ngày tới ngày
                //new DateTimeAxis(TimeSpan.FromDays(1), date => "Day " + date.ToString("dd"))
            };
        }

        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
    }


}
