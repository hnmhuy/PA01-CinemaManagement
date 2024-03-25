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
    public sealed partial class RoleDialogContent : Page
    {

        public string RoleName { get; set; }
        public RoleDialogContent(string roleName)
        {
            this.InitializeComponent();
            this.RoleName = roleName;
            RoleNameTextBox.Text = roleName;
        }

        private void RoleName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.RoleName = RoleNameTextBox.Text;   
        }
    }
}
