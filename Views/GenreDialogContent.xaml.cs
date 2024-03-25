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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GenreDialogContent : Page
    {
        public string GenreName { get; set; }   
        public GenreDialogContent(string genreName)
        {
            this.InitializeComponent();
            this.GenreName = genreName;
            GenreNameTextBox.Text = genreName;
        }

        private void GenreNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update the GenreName property when the text changes  
            GenreName = GenreNameTextBox.Text;
        }
    }
}
