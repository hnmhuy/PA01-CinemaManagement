using CinemaManagement.Models;
using CinemaManagement.WindowViews;
using CinemaManagement.ViewModels;
using CommunityToolkit.WinUI.UI.Controls;
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
using System.Diagnostics;
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
    public sealed partial class MoviesPage : Page, INotifyPropertyChanged
    {

        public MoviePageViewModel ViewModel { get; set; }
        public MovieViewModel movieViewModel { get; set; }
        public GenreViewModel genreViewModel { get; set; }
        public MoviesPage()
        {
            this.InitializeComponent();
            var _context = new DbCinemaManagementContext();

            movieViewModel = new MovieViewModel(_context);
            genreViewModel = new GenreViewModel(_context);

            ViewModel = new MoviePageViewModel(movieViewModel, genreViewModel);
            DataContext = ViewModel;
            //DataContext = movieViewModel; // Setting DataContext for movie-related elements
            //DataContext = genreViewModel; // Setting 

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void Button_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }

        private void MovieDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = sender as DataGrid;
            Debug.WriteLine(data.SelectedIndex);
            Debug.WriteLine(data.SelectedItem);
            (data.DataContext as MovieViewModel).SelectedMovie = data.SelectedItem as MovieCommand;

            //if (data != null)
            //{
            //    Debug.WriteLine(data.movie.Title);
            //}
            Debug.WriteLine(sender);
            Debug.WriteLine(e);
        }
        private void GenreDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = sender as DataGrid;
            Debug.WriteLine(data.SelectedIndex);
            Debug.WriteLine(data.SelectedItem);
            (data.DataContext as GenreViewModel).SelectedGenre = data.SelectedItem as GenreCommand;

            //if (data != null)
            //{
            //    Debug.WriteLine(data.movie.Title);
            //}
            Debug.WriteLine(sender);
            Debug.WriteLine(e);
        }
        private void createNewWindow_Click(object sender, RoutedEventArgs e)
        {

            // Set the content of the window to the desired page
            var addMoviePage = new AddMovieWindows(); // Assuming AddMoviePage is the desired page
            // Activate and show the new window
            addMoviePage.Activate();
        }
    }
}
