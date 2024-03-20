using CinemaManagement.Models;
using CinemaManagement.ViewModels;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Display.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class RegisterPage : Page
    {

        public HyperlinkButton GoToLogin;
        public RegisterViewModel dataContext;   
        public RegisterPage()
        {
            this.InitializeComponent();
            GoToLogin = GoToLoginButton;
            dataContext = new RegisterViewModel();
            this.DataContext = dataContext;

            DOBInput.MinDate = dataContext.minDate;
            DOBInput.MaxDate = dataContext.maxDate;
        }

        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Gener changed");
            var data = sender as ComboBox;
            if (data != null)
            {
                Debug.WriteLine(data.SelectedItem as string);
                Debug.WriteLine(data.SelectedItem.ToString());
                Debug.WriteLine((DataContext as RegisterViewModel).Gender);
            }
        }
    }
}
