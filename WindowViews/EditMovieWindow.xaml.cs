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
using Microsoft.EntityFrameworkCore;
using CinemaManagement.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.WindowViews
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditMovieWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Movie Movie { get; set; }
        public GenreViewModel genreViewModel { get; set; }
        public PersonViewModel personViewModel { get; set; }
        public RoleViewModel roleViewModel { get; set; }
        public ObservableCollection<AgeCertificate> AgeCertificateList { get; set; }
        public static ObservableCollection<Genre> SelectedGenreListStatic { get; set; }

        private AgeCertificate _selectedAge;
        public AgeCertificate SelectedAge
        {
            get { return _selectedAge; }
            set
            {
                if (value != _selectedAge)
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

        private List<bool> _isCheckedGenreList;
        public List<bool> IsCheckedGenreList
        {
            get { return _isCheckedGenreList; }
            set
            {
                _isCheckedGenreList = value;
                OnPropertyChanged(nameof(IsCheckedGenreList));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public EditMovieWindow(Movie movie)
        {
            this.InitializeComponent();
            Movie = movie;
            var context = new DbCinemaManagementContext();
            genreViewModel = new GenreViewModel(context);
            personViewModel = new PersonViewModel(context);
            roleViewModel = new RoleViewModel(context);


            SelectedRolesList = new ObservableCollection<Role>();

            SelectedCelebritiesList = new ObservableCollection<Person>();

            getContributor(context, Movie);

            var AgeContext = context.AgeCertificates.FirstOrDefault(g => g.AgeCertificateId == Movie.AgeCertificateId);
            inputMovieAge.Content = AgeContext.RequireAge;

            inputMovieImdb.Text = Movie.Imdbrating.ToString();

            getPath(Movie);
            SetRichEditBoxText(Movie.Description);
            isBlockisGold(Movie);
            SetDatePickerYear((int)Movie.PublishYear);

            AgeCertificateList = GetAgeCertificates(context);

            

            SelectedGenreList = new ObservableCollection<Genre>(Movie.Genres.ToList());

            //var genreCheckBoxConverter = new GenreCheckBoxConverter();
            //genreCheckBoxConverter.SelectedGenreList = SelectedGenreList;
            SelectedGenreListStatic = SelectedGenreList;
            this.Closed += EditMovieWindow_Closed;



        }


        public void getContributor(DbCinemaManagementContext context, Movie movie)
        {
            var contributors = context.Contributors
                          .Where(c => c.MovieId == movie.MovieId)
                          .ToList();


            // Iterate over contributors to retrieve associated persons and roles
            foreach (var contributor in contributors)
            {
                // Add the associated person to the list
                SelectedCelebritiesList.Add(contributor.Person);

                // Add the associated role to the list
                SelectedRolesList.Add(contributor.Role);
            }


        }

        private void SetRichEditBoxText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                REBCustom.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, text);
            }
        }
        private void SetDatePickerYear(int year)
        {
            // Create a new DateTime object with the desired year
            DateTime newDate = new DateTime(year, 1, 1);

            // Assign the new DateTime object to the Date property of the DatePicker
            inputMoviePublishYear.Date = newDate;
        }


        public void getPath(Movie movie)
        {
            if(movie.PosterPath == null)
            {
                PickAPhotoOutputTextBlock.Text = "None";
            }
            else
            {
                string[] Path = movie.PosterPath.Split('/');
                PickAPhotoOutputTextBlock.Text = Path[Path.Length - 1];
            }

            if (movie.TrailerPath == null)
            {
                PickATrailerOutputTextBlock.Text = "None";
            }
            else
            {
                string[] Path = movie.TrailerPath.Split('/');
                PickATrailerOutputTextBlock.Text = Path[Path.Length - 1];
            }
        }

        private void isBlockisGold(Movie movie)
        {
            if (movie.IsBlockbuster)
            {
                checkboxBlock.IsChecked = true;
            }
            else
            {
                checkboxBlock.IsChecked= false;
            }
            if (movie.IsGoldenHour)
            {
                checkboxGolden.IsChecked = true;
            }
            else
            {
                checkboxGolden.IsChecked = false;

            }
        }

        //public EditMovieWindow()
        //{

        //    this.InitializeComponent();
        //    var context = new DbCinemaManagementContext();
        //    genreViewModel = new GenreViewModel(context);
        //    personViewModel = new PersonViewModel(context);
        //    roleViewModel = new RoleViewModel(context);

        //    AgeCertificateList = GetAgeCertificates(context);


        //    SelectedGenreList = new ObservableCollection<Genre>(Movie.Genres.ToList());
        //    SelectedRolesList = new ObservableCollection<Role>();

        //    SelectedCelebritiesList = new ObservableCollection<Person>();

        //}

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
                    // Get the folder where the poster will be stored
                    StorageFolder posterFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets\\Images\\Poster");

                    // Create a new file in the poster folder with the same name as the selected file
                    StorageFile posterFile = await file.CopyAsync(posterFolder, file.Name, NameCollisionOption.ReplaceExisting);

                    // Optionally, you can store the path of the uploaded poster file for later use
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

        public void GenreCheckBox_Check(object sender, RoutedEventArgs e)
        {
            CheckBox clickedCheckBox = sender as CheckBox;

            if (clickedCheckBox.IsChecked == true && clickedCheckBox.DataContext is GenreCommand GenreCommand)
            {
                //Debug.WriteLine("SELECTED: " + genre.GenreName);
                //Debug.WriteLine("SELECTED: " + genre.GenreId);
                if (!SelectedGenreList.Any(genre => genre.GenreId == GenreCommand.Genre.GenreId))
                {
                    SelectedGenreList.Add(new Genre { GenreName = GenreCommand.Genre.GenreName, GenreId = GenreCommand.Genre.GenreId });

                }    
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
            Role selectedRole = deleteButton.DataContext as Role;

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
        private void SaveChange_Click(object sender, RoutedEventArgs e)
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

            string trailerPath = string.Empty;
            if (PickATrailerOutputTextBlock.Text != null)
            {
                trailerPath = "/Assets/Videos/" + PickATrailerOutputTextBlock.Text;

            }

            using (var context = new DbCinemaManagementContext())
            {
                var allGenres = context.Genres.ToList();
                var existingMovie = context.Movies
                                     .Include(m => m.Genres)
                                     .Include(m => m.Contributors)
                                     .FirstOrDefault(m => m.MovieId == Movie.MovieId);

                if (existingMovie != null)
                {
                    // Update the properties of the existing movie
                    existingMovie.Title = title;
                    existingMovie.Duration = duration;
                    existingMovie.PublishYear = publishYear;
                    existingMovie.Imdbrating = imdbRating;
                    existingMovie.Description = description;
                    existingMovie.PosterPath = posterPath;
                    existingMovie.TrailerPath = trailerPath;
                    existingMovie.IsBlockbuster = checkboxBlock.IsChecked ?? false;
                    existingMovie.IsGoldenHour = checkboxGolden.IsChecked ?? false;

                    existingMovie.Genres.Clear();
                    var filteredGenres = allGenres.Where(g => SelectedGenreList.Any(sg => sg.GenreId == g.GenreId)).ToList();
                    existingMovie.Genres = filteredGenres;                


                    existingMovie.Contributors.Clear();
                    context.Contributors.RemoveRange(existingMovie.Contributors);
                    if (SelectedCelebritiesList.Count == SelectedRolesList.Count)
                    {
                        // Iterate over the selected celebrities and roles to create contributors
                        for (int i = 0; i < SelectedCelebritiesList.Count; i++)
                        {
                            // Create a new Contributor object
                            Contributor contributor = new Contributor
                            {
                                Movie = existingMovie,
                                PersonId = SelectedCelebritiesList[i].PersonId,
                                RoleId = SelectedRolesList[i].RoleId
                            };
                            // Add the contributor to the movie's contributors collection
                            existingMovie.Contributors.Add(contributor);
                        }
                    }

                }
                else
                {
                    // Handle the case where the number of celebrities does not match the number of roles
                    Console.WriteLine("Error: The number of celebrities does not match the number of roles.");
                    return;
                }
                context.SaveChanges();
                Debug.WriteLine("Movie saved successfully.");
                
                
                this.Close();
            }



            Debug.WriteLine("Movie saved successfully.");


        }

        public event EventHandler<CancelEventArgs> WindowClosed;
        private void EditMovieWindow_Closed(object sender, Microsoft.UI.Xaml.WindowEventArgs e)
        {
            // Raise the WindowClosed event when the window is closed
            WindowClosed?.Invoke(this, new CancelEventArgs(false));
        }
    }


    public class GenreViewCheckBoxEditConverter : IValueConverter
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

    public class GenreCheckBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            if (value is GenreCommand genre && EditMovieWindow.SelectedGenreListStatic != null)
            {
                return EditMovieWindow.SelectedGenreListStatic.Any(g => g.GenreId == genre.Genre.GenreId);
            }
            return false;
            // Return false if any condition fails


        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
