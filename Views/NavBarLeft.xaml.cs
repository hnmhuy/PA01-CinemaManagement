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
    public sealed partial class NavBarLeft : Page
    {
        private Border _appTitleBar;

        private Frame contentFrame;

        private NavigationView navBar;

        public NavigationView NavBar
        {
            get { return navBar; }
            set { navBar = value; }
        }

        public Frame ContentFrame
        {
            get { return contentFrame; }
            set { contentFrame = value; }
        }


        public Border appTitleBar { get; private set; }

        public NavBarLeft()
        {
            InitializeComponent();
            _appTitleBar = TitleBar;
            contentFrame = MainContent;
            navBar = NavigationControl_Left;
        }

        void NavigationControl_Left_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            if (args.DisplayMode == NavigationViewDisplayMode.Minimal)
            {
                _appTitleBar.Margin = new Thickness(96, 12, 0, 0);
            }
            else
            {
                _appTitleBar.Margin = new Thickness(48, 12, 0, 0);
            }
        }
    }
}
