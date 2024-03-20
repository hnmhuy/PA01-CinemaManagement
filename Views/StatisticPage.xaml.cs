using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
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
    public sealed partial class StatisticPage : Page
    {
        public StatisticPage()
        {
            this.InitializeComponent();
        }



        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hide all DatePickers
            DayDatePicker.Visibility = Visibility.Collapsed;
            WeekDatePicker.Visibility = Visibility.Collapsed;
            MonthDatePicker.Visibility = Visibility.Collapsed;
            YearDatePicker.Visibility = Visibility.Collapsed;

            // Show the selected DatePicker
            string selectedMode = (string)ModeComboBox.SelectedItem;
            switch (selectedMode)
            {
                case "Day to day":
                    DayDatePicker.Visibility = Visibility.Visible;
                    break;
                case "Week":
                    WeekDatePicker.Visibility = Visibility.Visible;
                    break;
                case "Month":
                    MonthDatePicker.Visibility = Visibility.Visible;
                    break;
                case "Year":
                    YearDatePicker.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
