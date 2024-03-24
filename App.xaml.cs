using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CinemaManagement.WindowViews;
using CinemaManagement.ViewModels;
using CinemaManagement.Models;
using LiveChartsCore.Themes;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 
        private Window _mainWindow;
        public Window MainWindow => _mainWindow;   
        public bool IsClosedFromAuthenticateWindow { get; set; }
        private (bool, int, string?) formerData;
        public App()
        {
            this.InitializeComponent();
            IsClosedFromAuthenticateWindow = false;

            (Application.Current as App).RequestedTheme = ApplicationTheme.Light;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //var m_window = new MainWindow(1);
            //authenticateWindow = new AuthenticateWindow(); 
            //// Set the size of the window
            //authenticateWindow.Activate();

            //AddMOvie 

            //var m_windown = new AddMovieWindows();
            //m_window.Activate();


            // For debugging
            //AuthenticationControl.DestroySession();

            formerData = AuthenticationControl.RestoreSession();
            EnsureWindow();
            _mainWindow.Closed += (sender, e) =>
            {
                if (IsClosedFromAuthenticateWindow)
                {
                    Debug.WriteLine("Closed from AuthenticateWindow");
                    formerData = AuthenticationControl.RestoreSession();
                    EnsureWindow();
                    IsClosedFromAuthenticateWindow = false;
                }
            };

        }

        private void EnsureWindow()
        {
            if (formerData.Item1)
            {
                var uid = formerData.Item2;
                DbCinemaManagementContext context = new DbCinemaManagementContext();
                var user = context.Accounts.Where(u => u.AccountId == uid).FirstOrDefault();
                if (user != null && user.IsAdmin)
                {
                    _mainWindow = new AdminWindow();
                }
                else
                {
                    _mainWindow = new CustomerWindow();
                }
            } else
            {
                _mainWindow = new CustomerWindow();
            }
            _mainWindow.Activate();
            if (_mainWindow.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = _mainWindow is AdminWindow ? ElementTheme.Light : ElementTheme.Dark;
            }
        }

    }
}
