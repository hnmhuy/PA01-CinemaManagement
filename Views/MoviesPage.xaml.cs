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
using System.Threading.Tasks;
using Windows.Media.Capture;

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

            this.Loaded += MoviePage_Loaded;

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
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            // Find the parent DataGridRow
            var dataGridRow = FindVisualParent<DataGridRow>(button);

            // Extract the DataContext 
            if (dataGridRow != null)
            {
                var selectedMovie = dataGridRow.DataContext as MovieCommand;
                if (selectedMovie != null)
                {
                    //Debug.WriteLine("Selected Movie: " + selectedMovie.movie.Title); // Debugging statement

                    // Instantiate the EditMovieWindow
                    EditMovieWindow editMovieWindow = new EditMovieWindow(selectedMovie.movie);
                    //editMovieWindow.WindowClosed += EditMovieWindow_Closed;
                    // Show the EditMovieWindow
                    //this.Visibility = Visibility.Collapsed;
                    //Frame.Navigate(typeof(EditMovieWindow), selectedMovie.movie);
                    
                    editMovieWindow.Activate();

                }
                else
                {
                    Debug.WriteLine("DataContext of DataGridRow is not a Movie object."); // Debugging statement
                }
            }
            else
            {
                Debug.WriteLine("Parent DataGridRow not found."); // Debugging statement
            }

        }

        // Helper method to find visual parent of a specific type
        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        
        
        }

        private void MoviePage_Loaded(object sender, RoutedEventArgs e)
        {
            //ViewModel.RefreshData();
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Create new genre";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new GenreDialogContent("");
            dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
            _ = dialog.ShowAsync();
        }

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Debug.WriteLine("Primary button clicked");
            var dialog = sender as ContentDialog;
            var dialogContent = dialog.Content as GenreDialogContent;
            Debug.WriteLine(dialogContent.GenreName);
            ViewModel.CreateGenre(dialogContent.GenreName);
        }

        private void EditGenreBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Create new genre";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;            
            dialog.Content = new GenreDialogContent(ViewModel.GenresList.SelectedGenre.Genre.GenreName);
            dialog.PrimaryButtonClick += Dialog_ConfirmEdit;
            _ = dialog.ShowAsync();
        }

        private void Dialog_ConfirmEdit(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var dialog = sender as ContentDialog;
            var dialogContent = dialog.Content as GenreDialogContent;
            string newName = dialogContent.GenreName;
            ViewModel.GenresList.UpdateGenre(newName);
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //   base.OnNavigatedTo(e);
        //    ViewModel.RefreshData();
        //}
        //private void EditMovieWindow_Closed(object sender,EventArgs e)
        //{
        //    // Refresh data in the MoviesPage when the EditMovieWindow is closed
        //    ViewModel.RefreshData();
        //}
    }
}
