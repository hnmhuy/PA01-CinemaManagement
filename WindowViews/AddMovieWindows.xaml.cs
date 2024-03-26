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
using Windows.Security.Cryptography.Certificates;
using System.Security.Policy;
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

        public GenreViewModel genreViewModel {  get; set; }
        public PersonViewModel personViewModel { get; set; }
        public RoleViewModel roleViewModel { get; set; }
        public ObservableCollection<AgeCertificate> AgeCertificateList {  get; set; }

        private AgeCertificate _selectedAge;
        public AgeCertificate SelectedAge
        {
            get { return _selectedAge; }
            set
            {
                if(value != _selectedAge)
                {
                    _selectedAge = value;
                    OnPropertyChanged();
                }
            }
        }

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
        private ObservableCollection<Person> _selectedCebritiesList;
        public ObservableCollection<Person> SelectedCelebritiesList
        {
            get { return _selectedCebritiesList; }
            set
            {
                if (_selectedCebritiesList != value)
                {
                    _selectedCebritiesList = value;
                    OnPropertyChanged();
                }
            }
        }
        private ObservableCollection<Role> _selectedRolesList;
        public ObservableCollection<Role> SelectedRolesList
        {
            get { return _selectedRolesList; }
            set
            {
                if (_selectedRolesList != value)
                {
                    _selectedRolesList = value;
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

        public int newMovieId { get; set; }

        public AddMovieWindows()
        {
            newMovieId = -1;
            this.InitializeComponent();
            var context = new DbCinemaManagementContext();
            genreViewModel = new GenreViewModel(context);
            personViewModel = new PersonViewModel(context);
            roleViewModel = new RoleViewModel(context);

            AgeCertificateList = GetAgeCertificates(context);


            SelectedGenreList = new ObservableCollection<Genre>();
            SelectedRolesList = new ObservableCollection<Role>();

            SelectedCelebritiesList = new ObservableCollection<Person>();

        }

        public ObservableCollection<AgeCertificate> GetAgeCertificates(DbCinemaManagementContext context)
        {
            ObservableCollection<AgeCertificate> ageCertificates = new ObservableCollection<AgeCertificate>();
            var Ages = context.AgeCertificates.ToList();
            foreach (var ageCertificate in Ages)
            {
                AgeCertificate certificate = new AgeCertificate(); // Instantiate AgeCertificate object
                certificate.RequireAge = ageCertificate.RequireAge; // Set the required age
                ageCertificates.Add(certificate);
            }
            return ageCertificates;
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
                PickAPhotoOutputTextBlock.Text = file.Name;
                try
                {
                    string folderPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\Images\\Poster";

                    // Checking if the folder exist or not
                    if (!Directory.Exists(folderPath))
                    {
                        // If the folder does not exist, create it
                        Directory.CreateDirectory(Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\Images");
                        Directory.CreateDirectory(Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\Images\\Poster");
                    }

                    // Copy the selected file to the poster folder
                    StorageFile posterFile = await file.CopyAsync(StorageFolder.GetFolderFromPathAsync(folderPath).AsTask().Result, file.Name, NameCollisionOption.ReplaceExisting);
                    string posterPath = posterFile.Path;
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during file copying
                    PickAPhotoOutputTextBlock.Text = "Error: " + ex.Message;
                }
            }
            else
            {
                PickAPhotoOutputTextBlock.Text = "Operation cancelled.";
            }

        }

        private async void PickATrailerButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new FileOpenPicker
            FileOpenPicker openPicker = new FileOpenPicker();
            // Set properties for the FileOpenPicker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".avi");
            openPicker.FileTypeFilter.Add(".mkv");

            // Show the FileOpenPicker and capture the picked file
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Display the file name in the associated TextBlock
                PickATrailerOutputTextBlock.Text = file.Name;
                // You can also store the file reference for later use
                // For example: Save the StorageFile object to upload it later
                // var trailerFile = file;
            }
            else
            {
                // User canceled the operation
                PickATrailerOutputTextBlock.Text = "Operation canceled.";
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

            if (clickedCheckBox.IsChecked == true && clickedCheckBox.DataContext is GenreCommand GenreCommand)
            {
                //Debug.WriteLine("SELECTED: " + genre.GenreName);
                //Debug.WriteLine("SELECTED: " + genre.GenreId);
                SelectedGenreList.Add(new Genre { GenreName = GenreCommand.Genre.GenreName, GenreId = GenreCommand.Genre.GenreId });
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

            if (clickedCheckBox.IsChecked == false && clickedCheckBox.DataContext is GenreCommand unselectedGenre)
            {
                //Debug.WriteLine("UNSELECTED: " + unselectedGenre.GenreName);
                var genreToRemove = SelectedGenreList.FirstOrDefault(g => g.GenreId == unselectedGenre.Genre.GenreId);
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

        private void ChooseCelebrity_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CelebritySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                PersonCommand selectedPerson = e.AddedItems[0] as PersonCommand; // Assuming 'Person' is the type of items in your GridView
                if (selectedPerson != null)
                {
                    // Add the selected person to the SelectedCelebritiesList
                    SelectedCelebritiesList.Add(selectedPerson.Person);
                    Debug.WriteLine(SelectedCelebritiesList.Count);
                    foreach (var item in SelectedCelebritiesList)
                    {
                        Debug.WriteLine(item.Fullname + " " + item.PersonId + " " + item.AvatarPath);
                    }    
                }
            }
        }
        private void AddCelebrityToList()
        {

        }

        private void RoleDropdown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RoleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                RoleCommand selectedRole = e.AddedItems[0] as RoleCommand; // Assuming 'Role' is the type of items in your GridView
                if (selectedRole != null)
                {
                    // Add the selected person to the SelectedCelebritiesList
                    SelectedRolesList.Add(selectedRole.Role);
                    Debug.WriteLine(SelectedRolesList.Count);
                    foreach (var item in SelectedRolesList)
                    {
                        Debug.WriteLine(item.RoleName + " " + item.RoleId);
                    }
                }
            }
        }

        private void DeleteSelectedCelebrity_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;

            // Get the corresponding celebrity from the DataContext of the button
            Person selectedPerson = deleteButton.DataContext as Person;

            // Remove the selected celebrity from the SelectedCelebritiesList
            if (selectedPerson != null)
            {
                SelectedCelebritiesList.Remove(selectedPerson);
                Debug.WriteLine("DELETE PERSON");
            }
            Debug.WriteLine(SelectedCelebritiesList.Count);
            foreach (var item in SelectedCelebritiesList)
            {

                Debug.WriteLine(item.Fullname + " " + item.PersonId + " " + item.AvatarPath);
            }
        }

        private void DeleteSelectedRole_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;

            // Get the corresponding celebrity from the DataContext of the button
            Role selectedRole= deleteButton.DataContext as Role;

            // Remove the selected celebrity from the SelectedCelebritiesList
            if (selectedRole != null)
            {
                SelectedRolesList.Remove(selectedRole);
            }
                Debug.WriteLine(SelectedRolesList.Count);
            foreach (var item in SelectedRolesList)
            {
                Debug.WriteLine(item.RoleName + " " + item.RoleId);
            }
        }
        private void AgeGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                // Assuming each item in AgeCertificateList is of type AgeCertificate
                SelectedAge = e.AddedItems[0] as AgeCertificate;
                inputMovieAge.Content = SelectedAge.RequireAge;
                Debug.WriteLine($"Selected Age: {SelectedAge.RequireAge}");
            }
        }
        private void SaveMovie_Click(object sender, RoutedEventArgs e)
        {
            // Fetch data from input fields
            string title = inputMovieTitle.Text;
            int duration = (int)inputMovieDuration.Value;
            string ageCertificate = inputMovieAge.Content.ToString();
            int publishYear = inputMoviePublishYear.Date.Year;


            string description = string.Empty;
            REBCustom.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out description);

            //double imdbRating = string.IsNullOrEmpty(inputMovieImdb.Text) ? null : double.Parse(inputMovieImdb.Text);
            double imdbRating = double.Parse(inputMovieImdb.Text);

            string posterPath = string.Empty;
            if (PickAPhotoOutputTextBlock.Text != null)
            {
                posterPath = "/Assets/Images/Poster/" + PickAPhotoOutputTextBlock.Text;

            }

            string trailerPath = "/Assets/Videos/dune_part_two.mp4";
            //if (PickATrailerOutputTextBlock.Text != null)
            //{
            //    trailerPath = "/Assets/Videos/" + PickATrailerOutputTextBlock.Text;

            //}

            // Fetch genres from the database based on selected genre IDs
            Debug.WriteLine(SelectedAge.RequireAge + " - " + SelectedAge.AgeCertificateId);
            var context = new DbCinemaManagementContext();
            var ageCertificateId = context.AgeCertificates.Where(ag => ag.RequireAge == SelectedAge.RequireAge).FirstOrDefault().AgeCertificateId;
            var allGenres = context.Genres.ToList();
            var filteredGenres = allGenres.Where(g => SelectedGenreList.Any(sg => sg.GenreId == g.GenreId)).ToList();
            Movie newMovie = new Movie
            {
                Title = title,
                Duration = duration,
                PublishYear = publishYear,
                Imdbrating = imdbRating,
                Description = description,
                PosterPath = posterPath,
                TrailerPath = trailerPath,
                Genres = filteredGenres,
                AgeCertificateId = ageCertificateId,
                IsBlockbuster = checkboxBlock.IsChecked ?? false,
                IsGoldenHour = checkboxGolden.IsChecked ?? false
            };
            context.Movies.Add(newMovie);


            if (SelectedCelebritiesList.Count == SelectedRolesList.Count)
            {
                // Iterate over the selected celebrities and roles to create contributors
                for (int i = 0; i < SelectedCelebritiesList.Count; i++)
                {
                    // Create a new Contributor object
                    Contributor contributor = new Contributor
                    {
                        Movie = newMovie,
                        PersonId = SelectedCelebritiesList[i].PersonId,
                        RoleId = SelectedRolesList[i].RoleId
                    };
                    // Add the contributor to the movie's contributors collection
                    newMovie.Contributors.Add(contributor);
                }
            }
            else
            {
                // Handle the case where the number of celebrities does not match the number of roles
                Console.WriteLine("Error: The number of celebrities does not match the number of roles.");
                return;
            }
            context.SaveChanges();

            newMovieId = newMovie.MovieId;

            this.Close();

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
