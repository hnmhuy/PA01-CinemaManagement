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
using Windows.ApplicationModel.DataTransfer; 
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI;
using WinRT.Interop;
using Windows.UI;
using CinemaManagement.Models;
using CinemaManagement.ViewModels;
using System.Diagnostics;
using System.Collections.ObjectModel;   
using System.ComponentModel;
using System.Runtime.CompilerServices;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.WindowViews
{


    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddMovieWindows : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Genre> _selectedGenreList;
        public ObservableCollection<Genre> SelectedGenreList
        {
            get { return _selectedGenreList; }
            set
            {
                if (_selectedGenreList != value)
                {
                    _selectedGenreList = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public AddMovieWindows()
        {
            this.InitializeComponent();
            SelectedGenreList = new ObservableCollection<Genre>();

        }

        private async void PickAPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous returned file name, if it exists, between iterations of this scenario
            PickAPhotoOutputTextBlock.Text = "";

            // Create a file picker
            var openPicker = new FileOpenPicker();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Initialize the file picker with the window handle (HWND)
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);


            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                PickAPhotoOutputTextBlock.Text = "Picked photo: " + file.Name;
            }
            else
            {
                PickAPhotoOutputTextBlock.Text = "Operation cancelled.";
            }

        }
        private void Menu_Opening(object sender, object e)
        {
            CommandBarFlyout myFlyout = sender as CommandBarFlyout;
            if (myFlyout.Target == REBCustom)
            {
                AppBarButton myButton = new AppBarButton();
                myButton.Command = new StandardUICommand(StandardUICommandKind.Share);
                myFlyout.PrimaryCommands.Add(myButton);
            }
        }

        private void REBCustom_Loaded(object sender, RoutedEventArgs e)
        {
            REBCustom.SelectionFlyout.Opening += Menu_Opening;
            REBCustom.ContextFlyout.Opening += Menu_Opening;
        }

        private void REBCustom_Unloaded(object sender, RoutedEventArgs e)
        {
            REBCustom.SelectionFlyout.Opening -= Menu_Opening;
            REBCustom.ContextFlyout.Opening -= Menu_Opening;
        }

        private void blockGoldenCheckbox_Click(object sender, RoutedEventArgs e)
        {
            string selectedTypesText = string.Empty;
            bool isBlockbusterChecked = checkboxBlock.IsChecked == true;
            bool isGoldenHourChecked = checkboxGolden.IsChecked == true;

            if (isBlockbusterChecked && isGoldenHourChecked)
            {
                typesList.Text = "You've selected a special movie that's both a blockbuster hit and airing during the golden hour!";
            }
            else if (isBlockbusterChecked)
            {
                typesList.Text = "This is a Blockbuster Movie";
            }
            else if (isGoldenHourChecked)
            {
                typesList.Text = "This movie is airing during the Golden Hour";
            }
            else
            {
                typesList.Text = "No special type selected";
            }
        }
        //private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    string colorName = e.AddedItems[0].ToString();
        //    Color color = Colors.Black;
        //    switch (colorName)
        //    {
        //        case "Yellow":
        //            color = Colors.Yellow;
        //            break;
        //        case "Green":
        //            color = Colors.Green;
        //            break;
        //        case "Blue":
        //            color = Colors.Blue;
        //            break;
        //        case "Red":
        //            color = Colors.Red;
        //            break;
        //    }
        //    colorRectangle.Fill = new SolidColorBrush(color);
        //}

        public void GenreCheckBox_Check(object sender, RoutedEventArgs e)
        {
            CheckBox clickedCheckBox = sender as CheckBox;

            if (clickedCheckBox.IsChecked == true && clickedCheckBox.DataContext is Genre genre)
            {
                //Debug.WriteLine("SELECTED: " + genre.GenreName);
                //Debug.WriteLine("SELECTED: " + genre.GenreId);
                SelectedGenreList.Add(new Genre { GenreName = genre.GenreName, GenreId = genre.GenreId });
                Debug.WriteLine(SelectedGenreList.Count);
            }
            foreach (var item in SelectedGenreList)
            {
                Debug.WriteLine("Selected Genre: " + item.GenreName + ", Genre ID: " + item.GenreId);
            }

        }

        public void GenreCheckBox_UnCheck(object sender, RoutedEventArgs e)
        {
            CheckBox clickedCheckBox = sender as CheckBox;

            if (clickedCheckBox.IsChecked == false && clickedCheckBox.DataContext is Genre unselectedGenre)
            {
                //Debug.WriteLine("UNSELECTED: " + unselectedGenre.GenreName);
                var genreToRemove = SelectedGenreList.FirstOrDefault(g => g.GenreId == unselectedGenre.GenreId);
                if (genreToRemove != null)
                {
                    SelectedGenreList.Remove(genreToRemove);
                }
            }
            foreach (var item in SelectedGenreList)
            {
                Debug.WriteLine("UnSelected Genre: " + item.GenreName + ", Genre ID: " + item.GenreId);
            }
            Debug.WriteLine(SelectedGenreList.Count);

        }




    }

    public class GenreViewCheckBoxConverter : IValueConverter
    {
        public class CombinedItem
        {
            public String GenreName { get; set; }
            public String Icon { get; set; }
        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<Genre> genres)
            {
                var combinedItems = new List<CombinedItem>();
                for (int i = 0; i < genres.Count; i++)
                {
                    combinedItems.Add(new CombinedItem
                    {
                        GenreName = genres[i].GenreName,
                        Icon = (i < genres.Count - 1) ? "\uF83F" : string.Empty
                    });
                }
                return combinedItems;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
