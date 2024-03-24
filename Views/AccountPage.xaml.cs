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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Windows.Forms;
using Application = Microsoft.UI.Xaml.Application;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        public AccountViewModel viewModel { get; set; }
        public Page CurrPage { get; set; }
        public AccountPage()
        {
            this.InitializeComponent();
            viewModel = new AccountViewModel();
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.DataContext = viewModel;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.IsAuthenticated))
            {
                if (viewModel.IsAuthenticated)
                {
                    if (viewModel.returnValue.Item2.IsAdmin)
                    {
                        var window = (Application.Current as App).MainWindow;
                        (Application.Current as App).IsClosedFromAuthenticateWindow = true;
                        window.Close();
                    }
                }
            }
        }

        private void resetPasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            AccountDetail.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            ResetPasswordPage.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            AccountDetail.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            ResetPasswordPage.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;

        }

        private void savePasswordBtn_Click(object sender, RoutedEventArgs e)
        {
            // Your event handling logic here
        }
        
            //var result = (this.DataContext as AccountViewModel).ChangePassword();
            //if (result.Item1)
            //{
            //    var dialog = new ContentDialog
            //    {
            //        Title = "Success",
            //        Content = result.Item2,
            //        CloseButtonText = "Ok"
            //    };
            //    dialog.XamlRoot = this.XamlRoot;
            //    _ = await dialog.ShowAsync();
            //}
            //else
            //{
            //    var dialog = new ContentDialog
            //    {
            //        Title = "Error",
            //        Content = result.Item2,
            //        CloseButtonText = "Ok"
            //    };
            //    dialog.XamlRoot = this.XamlRoot;
            //    _ = await dialog.ShowAsync();
            //}
        
    }
}
