using CinemaManagement.Models;
using CinemaManagement.ViewModels;
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.WindowViews
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShowTimeModifyWindow : Window
    {
        private ShowtimeModifyViewModel _viewModel;
        public (bool, string, int) returnVal;
        public ShowTimeModifyWindow(ShowTime showtime)
        {
            this.InitializeComponent();
            this.Content = new ShowTimeModifierPage();
            _viewModel = (Content as ShowTimeModifierPage)._viewModel as ShowtimeModifyViewModel;
            _viewModel.PropertyChanged += OnSaveReturnChange;
        }

        private void OnSaveReturnChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.IsSaved))
            {
                if ((Content as ShowTimeModifierPage)._viewModel.IsSaved)
                {
                    returnVal = _viewModel.returnValue;
                    this.Close();
                }
            }
        }
    }
}
