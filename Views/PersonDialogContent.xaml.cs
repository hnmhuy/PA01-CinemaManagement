using CinemaManagement.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonDialogContent : Page
    {

        public Person person { get; set; }
        public string AvatarPath { get; set; }
        public PersonDialogContent(Person _person)
        {
            this.InitializeComponent();
            if (_person != null)
            {
                this.person = _person;
                string fullPath = Path.GetFullPath(person.AvatarPath);
                PersonPicture.ProfilePicture = new BitmapImage(new Uri(fullPath, UriKind.Relative));
            } else
            {
                this.person = new Person() { AvatarPath = "", Biography= "", Fullname = ""};
            }
        }

        private async void SelectAvatar_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle((Application.Current as App).MainWindow);

            // Initialize the file picker with the window handle (HWND)
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);


            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {    
                
                try
                {
                    // Get the folder where the poster will be
                    string folderPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\Images\\Person";

                    // Checking if the folder exist or not
                    if (!Directory.Exists(folderPath))
                    {
                        // If the folder does not exist, create it
                        Directory.CreateDirectory(Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\Images");
                        Directory.CreateDirectory(Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\Images\\Person");
                    }

                    // Copy the selected file to the poster folder
                    StorageFile posterFile = await file.CopyAsync(StorageFolder.GetFolderFromPathAsync(folderPath).AsTask().Result, file.Name, NameCollisionOption.ReplaceExisting);


                    //StorageFolder posterFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(folderPath);

                    //// Create a new file in the poster folder with the same name as the selected file
                    //StorageFile posterFile = await file.CopyAsync(posterFolder, file.Name, NameCollisionOption.ReplaceExisting);

                    // Optionally, you can store the path of the uploaded poster file for later use
                    string posterPath = posterFile.Path;
                    person.AvatarPath = "/Assets/Images/Person/" + file.Name;
                    // Load the image from the uploaded file
                    PersonPicture.ProfilePicture = new BitmapImage(new Uri(folderPath + "\\" + file.Name, UriKind.Relative));
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during file copying
                    Debug.WriteLine($"Error copying file: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine("User cancelled the file picker.");
            }
        }
    }
}
