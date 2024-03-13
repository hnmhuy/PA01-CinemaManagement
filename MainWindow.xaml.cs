using CinemaManagement.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        private int role = 0;
        public MainWindow(int role)
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            Page page = null;

            if (role == 0)
            {
                page = new NavBarTop();
                this.Content = page;
                NavigationHelper navigationHelper = new NavigationHelper(
                    (page as NavBarTop).NavBar,
                    (page as NavBarTop).ContentFrame,
                    typeof(BrowsePage));
            }
            else if (role == 1)
            {
                page = new NavBarLeft();
                this.Content = page;
                SetTitleBar((page as NavBarLeft).appTitleBar);
                NavigationHelper navigationHelper = new NavigationHelper(
                    (page as NavBarLeft).NavBar,
                    (page as NavBarLeft).ContentFrame,
                    typeof(DashboardPage));
            }
        }
    }

    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double compactPaneLength)
            {
                return new Thickness(0, compactPaneLength, 0, 0);
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
