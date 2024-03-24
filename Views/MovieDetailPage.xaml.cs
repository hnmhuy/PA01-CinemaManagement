using CinemaManagement.Models;
using CinemaManagement.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MovieDetailPage : Page
    {
        public static int movieId { get; set; }
        private bool isTrailerVisible = false;
        private ScrollPresenter scrollPresenter;
        private Frame contentFrame;
        private const int rowSpacing = 24;
        private MovieDetailViewModel viewModel;
        public bool isCheckOperation = false;

        public MovieDetailPage()
        {
            this.InitializeComponent();
            // Limit the date picker to only show dates in the future
            ShowDatePicker.MinYear = DateTime.Now;
            ShowDatePicker.Date = DateTime.Now;
            Debug.WriteLine("Movie detail page with id=" + movieId);
            viewModel = new MovieDetailViewModel(movieId);
            this.DataContext = viewModel;
            viewModel.selectedDate = DateTime.Now;
            viewModel.PropertyChanged += ViewModelDataChange;
        }


        private void ViewModelDataChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.MaxCol))
            {
                Debug.WriteLine("Max column change to " + viewModel.MaxCol);
                SeatLayout.MaximumRowsOrColumns = viewModel.MaxCol;
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(SeatContainer) - 1; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(SeatContainer, i);
                    if (child != null && child is ToggleButton)
                    {
                        (child as ToggleButton).IsChecked = false;
                    }
                }
            }
            else if (e.PropertyName == nameof(viewModel.NotifyCode))
            {
                string title = "Notification";
                Debug.WriteLine(viewModel.NotifyMessage);
                ContentDialog dialog = new ContentDialog
                {
                    Title = title,
                    Content = viewModel.NotifyCode == MovieDetailViewModel.BOOKING_SUCCESS ? "Book succesfull" : "Book failed",
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.XamlRoot;
                _ = dialog.ShowAsync();
            }
        }

        // Override OnNavigatedTo method
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Debug.WriteLine("Navigate to MovieDetailPage with MovieId=" + movieId);
            ((this.Parent as Frame).Parent as ScrollPresenter).ScrollTo(0, 0, null);
        }

        private void TrailerButton_Click(object sender, RoutedEventArgs e)
        {
            if (scrollPresenter is null)
            {
                contentFrame = this.Parent as Frame;
                scrollPresenter = contentFrame.Parent as ScrollPresenter;
            }
            if (!isTrailerVisible && TrailerPlayer != null)
            {
                TrailerPlayer.MediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Videos/" + (this.DataContext as MovieDetailViewModel).CurrMovie.TrailerPath));
                TrailerContainer.Visibility = Visibility.Visible;
                scrollPresenter.ScrollTo(0, 0, null);
                TrailerPlayer.MediaPlayer.Play();
                isTrailerVisible = true;

            }
            else
            {
                isTrailerVisible = false;
                scrollPresenter.ScrollTo(0, 0, null);
                TrailerPlayer.MediaPlayer.Pause();
                TrailerContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void GoToShowtime_Click(object sender, RoutedEventArgs e)
        {
            if (scrollPresenter is null)
            {
                contentFrame = this.Parent as Frame;
                scrollPresenter = contentFrame.Parent as ScrollPresenter;
            }
            double offset = MovieDetailSection.RowDefinitions[1].ActualHeight + MovieDetailSection.RowDefinitions[0].ActualHeight + rowSpacing * 2;
            scrollPresenter.ScrollTo(0, offset, null);
            TrailerPlayer.MediaPlayer.Pause();
        }

        private void GoToShowtime_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Display the text
            txtGotoShowtime.Visibility = Visibility.Visible;
        }

        private void GoToShowtime_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            txtGotoShowtime.Visibility = Visibility.Collapsed;
        }

        private void InteractiveSeat_Checked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MovieDetailViewModel).AddTicket((sender as ToggleButton).DataContext as Ticket);
        }

        private void InteractiveSeat_Unchecked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MovieDetailViewModel).RemoveTicket((sender as ToggleButton).DataContext as Ticket);
        }

        private async void Voucher_Checked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(sender.GetType().ToString());
            var (res, message) = (this.DataContext as MovieDetailViewModel).AddVoucher((sender as CheckBox).DataContext as CheckBoxVoucher);
            if (!res && viewModel.allowDialog)
            {
                //(sender as CheckBox).IsChecked = false;
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = message,
                    CloseButtonText = "OK"
                };

                dialog.XamlRoot = this.XamlRoot;
                _ = await dialog.ShowAsync();
                isCheckOperation = true;
                (sender as CheckBox).IsChecked = false;
                isCheckOperation = false;
            }
        }

        private async void Voucher_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isCheckOperation) return;
            var (res, message) = (this.DataContext as MovieDetailViewModel).RemoveVoucher((sender as CheckBox).DataContext as CheckBoxVoucher);
            if (!res && viewModel.allowDialog)
            {
                //(sender as CheckBox).IsChecked = true;
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = message,
                    CloseButtonText = "OK"
                };
                dialog.XamlRoot = this.XamlRoot;
                _ = await dialog.ShowAsync();
                (sender as CheckBox).IsChecked = true;
            }
        }

        private void ShowDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            viewModel.selectedDate = new DateTime(e.NewDate.Year, e.NewDate.Month, e.NewDate.Day);
        }
    }

    public class VietnameseCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double)
            {
                return ((double)value).ToString("C0", new System.Globalization.CultureInfo("vi-VN"));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberSeatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "x" + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DiscountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null)
            {
                Debug.WriteLine(parameter.ToString());
                bool isPercent = Boolean.Parse(parameter.ToString());
                Debug.WriteLineIf(isPercent, "Is percent");
                Debug.WriteLineIf(!isPercent, "Isn't percent");
            }
            else Debug.WriteLine("Parameter is null");
            // Get type of value and write in Debug
            Debug.WriteLine(value.GetType().ToString());



            return "Discount";
            //bool isPercent = (bool)parameter;   
            //if (isPercent)
            //{
            //    return "Giảm " + (double)value + "%";
            //} else 
            //{
            //    return "Giảm " + ((double)value).ToString("C0", new System.Globalization.CultureInfo("vi-VN"));
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)value + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            bool negativeValue = bool.Parse(parameter.ToString());
            if (negativeValue)
            {
                return !(bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
