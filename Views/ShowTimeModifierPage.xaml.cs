using CinemaManagement.Models;
using CinemaManagement.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShowTimeModifierPage : Page
    {
        public ShowtimeModifyViewModel _viewModel;
        private int _screenWidth;
        private int _screenHeight;
        private ShowTime _showtime;
        private ObservableCollection<Movie> filteredMovies;

        public ShowTimeModifierPage(ShowTime showTime)
        {
            this.InitializeComponent();
            this._showtime = showTime;
            _viewModel = new ShowtimeModifyViewModel(showTime);
            this.DataContext = _viewModel;
            _viewModel.PropertyChanged += _viewModel_PropertyChanged;
            DatePicker.MinDate = DateTime.Now.AddDays(1);
            DatePicker.MaxDate = DateTime.Now.AddDays(8);
            if (showTime != null)
            {
                BindingIfShowtimeExist();
            }
        }

        private void BindingIfShowtimeExist()
        {
            DatePicker.Date = _viewModel.SelectedDate;  
            TimePicker.Time = _viewModel.SelectedTime;
            NumberRow.Text = _viewModel.NumberOfRows.ToString();
            NumberCol.Text = _viewModel.NumberOfColumns.ToString();
        }

        private void _viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.NumberOfColumns))
            {
                SeatLayout.MaximumRowsOrColumns = _viewModel.NumberOfColumns;
            }
            else if (e.PropertyName == nameof(_viewModel.IsSaved))
            {
                if (!_viewModel.IsSaved)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = _viewModel.returnValue.Item2,
                        CloseButtonText = "OK"
                    };
                    dialog.XamlRoot = this.Content.XamlRoot;
                    _ = dialog.ShowAsync();
                }
            }
        }

        private void NumberRow_TextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine("NumberRow_TextChanged");
            try
            {
                _viewModel.NumberOfRows = int.Parse(NumberRow.Text);
            }
            catch
            {
                _viewModel.NumberOfRows = 0;
            }
        }

        private void NumberCol_TextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine("NumberCol_TextChanged");
            try
            {
                _viewModel.NumberOfColumns = int.Parse(NumberCol.Text);
            }
            catch
            {
                _viewModel.NumberOfColumns = 0;
            }
        }

        private void MovieSelection_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = dataset;
                filteredMovies = new ObservableCollection<Movie>(_viewModel.MovieList.Where(x => x.Title.Contains(sender.Text)));
                sender.ItemsSource = filteredMovies;
            }
        }

        private void MovieSelection_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to get the selected item
            sender.Text = (args.SelectedItem as Movie).Title;
            _viewModel.SelectedMovie = args.SelectedItem as Movie;
        }

        private void DatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            // Update the selected date
            _viewModel.SelectedDate = new DateTime(args.NewDate.Value.Year, args.NewDate.Value.Month, args.NewDate.Value.Day);
        }

        private void TimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            // Update the selected time (timespan)
            _viewModel.SelectedTime = e.NewTime;
        }

        private void NormalPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _viewModel.NormalPrice = double.Parse(NormalPrice.Text);
            }
            catch
            {
                Debug.WriteLine("Invalid input");
            }
        }

        private void VIPPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _viewModel.VipPrice = double.Parse(VIPPrice.Text);
            }
            catch
            {
                Debug.WriteLine("Invalid input");
            }
        }

        private void InteractiveSeat_Checked(object sender, RoutedEventArgs e)
        {
            var seat = (sender as ToggleButton).DataContext as Ticket;
            // Find the ticket in the list and set IsVip to true
            var ticket = _viewModel.TicketList.FirstOrDefault(x => x.Row == seat.Row && x.Col == seat.Col);
            if (ticket != null)
            {
                ticket.IsVip = !ticket.IsVip;
            }

        }

        private void InteractiveSeat_Unchecked(object sender, RoutedEventArgs e)
        {
            var seat = (sender as ToggleButton).DataContext as Ticket;
            // Find the ticket in the list and set IsVip to true
            var ticket = _viewModel.TicketList.FirstOrDefault(x => x.Row == seat.Row && x.Col == seat.Col);
            if (ticket != null)
            {
                ticket.IsVip = !ticket.IsVip;
            }
        }
    }
}
