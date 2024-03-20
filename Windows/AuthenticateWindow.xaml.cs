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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuthenticateWindow : Window
    {
        public TextBox userName;
        public PasswordBox password;
        public Button LoginButton;
        public HyperlinkButton HyperlinkButton;
        public Type CurrentPage { get; set; }

        public AuthenticateWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            MainContent.Navigated += MainContent_Navigated;
            MainContent.Navigate(typeof(LoginPage));
        }

        private void MainContent_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainContent.Content is LoginPage)
            {
                HyperlinkButton = (MainContent.Content as LoginPage).GoToRegister;
                // Clear the event handler for the Click event.
            }
            else
            {
                HyperlinkButton = (MainContent.Content as RegisterPage).GoToLogin;
            }
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
       

    }
}
