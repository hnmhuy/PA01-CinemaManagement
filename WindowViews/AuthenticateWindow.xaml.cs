using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CinemaManagement.Views;
using Windows.Graphics;
using Microsoft.UI.Windowing;
using CinemaManagement.Models;
using CinemaManagement.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.WindowViews
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuthenticateWindow : Window
    {
        public HyperlinkButton HyperlinkButton;
        public Button LoginBtn;
        public Button RegisterBtn;
        public Type currType { get; set; }
        public Page CurrPage { get; set; }
        public (bool, Account, string) returnValue;

        public AuthenticateWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            MainContent.Navigated += MainContent_Navigated;
            MainContent.Navigate(typeof(LoginPage));
            CurrPage = MainContent.Content as Page;
        }

        private void MainContent_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainContent.Content is LoginPage)
            {
                HyperlinkButton = (MainContent.Content as LoginPage).GoToRegister;
                // Clear the event handler for the Click event.
                HyperlinkButton.Click -= HyperlinkButton_Click;
                LoginBtn = (MainContent.Content as LoginPage).LoginButton;
                LoginBtn.Click += LoginBtn_Click;
            }
            else
            {
                HyperlinkButton = (MainContent.Content as RegisterPage).GoToLogin;
                // Clear the event handler for the Click event.
                HyperlinkButton.Click -= HyperlinkButton_Click;
                RegisterBtn = (MainContent.Content as RegisterPage).RegisterButton;
                RegisterBtn.Click += RegisterBtn_Click;
            }
            CurrPage = MainContent.Content as Page;
            HyperlinkButton.Click += HyperlinkButton_Click;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainContent.Content is LoginPage)
            {
                MainContent.Navigate(typeof(RegisterPage));
            }
            else
            {
                MainContent.Navigate(typeof(LoginPage));
            }
        }

        // Function for login button
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainContent.Content is LoginPage)
            {
                CurrPage = MainContent.Content as LoginPage;
                (CurrPage.DataContext as LoginViewModel).Login();
                returnValue = (CurrPage.DataContext as LoginViewModel).value;
                if (returnValue.Item1)
                {
                    this.Close();
                }
                else
                {
                    // Show dialog
                    var dialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = returnValue.Item3,
                        CloseButtonText = "Ok"
                    };
                    dialog.XamlRoot = this.Content.XamlRoot;
                    ContentDialogResult result = dialog.ShowAsync().AsTask().Result;
                }
            }
        }

        // Function for register button
        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainContent.Content is RegisterPage)
            {
                CurrPage = MainContent.Content as RegisterPage;
                (CurrPage.DataContext as RegisterViewModel).Register();
                returnValue = (CurrPage.DataContext as RegisterViewModel).value;
                if (returnValue.Item1)
                {
                    this.Close();
                }
                else
                {
                    // Show dialog
                    var dialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = returnValue.Item3,
                        CloseButtonText = "Ok"
                    };
                    dialog.XamlRoot = this.Content.XamlRoot;
                    ContentDialogResult result = dialog.ShowAsync().AsTask().Result;
                }
            }
        }
    }
}
