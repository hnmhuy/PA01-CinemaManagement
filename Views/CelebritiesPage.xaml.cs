using CinemaManagement.Models;
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
    public sealed partial class CelebritiesPage : Page, INotifyPropertyChanged
    {
        public CelebritiesPageViewModel ViewModel { get; set; }
        public PersonViewModel personViewModel { get; set; }
        public RoleViewModel roleViewModel { get; set; }
        public CelebritiesPage()
        {
            this.InitializeComponent();
            var _context = new DbCinemaManagementContext();

            personViewModel = new PersonViewModel(_context);
            roleViewModel = new RoleViewModel(_context);

            ViewModel = new CelebritiesPageViewModel(personViewModel, roleViewModel);
            this.DataContext = ViewModel;
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

        private void PersonDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = sender as DataGrid;
            Debug.WriteLine(data.SelectedIndex);
            Debug.WriteLine(data.SelectedItem);
            (data.DataContext as PersonViewModel).SelectedPerson = data.SelectedItem as PersonCommand;

            //if (data != null)
            //{
            //    Debug.WriteLine(data.movie.Title);
            //}
            Debug.WriteLine(sender);
            Debug.WriteLine(e);
        }
        private void RoleDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = sender as DataGrid;
            Debug.WriteLine(data.SelectedIndex);
            Debug.WriteLine(data.SelectedItem);
            (data.DataContext as RoleViewModel).SelectedRole = data.SelectedItem as RoleCommand;

            //if (data != null)
            //{
            //    Debug.WriteLine(data.movie.Title);
            //}
            Debug.WriteLine(sender);
            Debug.WriteLine(e);
        }

        private void AddNewRole_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Create new role";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new RoleDialogContent("");
            dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
            _ = dialog.ShowAsync();

        }

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var dialog = sender as ContentDialog;
            var dialogContent = dialog.Content as RoleDialogContent;    
            var roleName = dialogContent.RoleName;
            Debug.WriteLine(roleName);
            ViewModel.RolesList.AddNewRole(roleName);
        }

        private void EditRole_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            Debug.WriteLine(ViewModel.RolesList.SelectedRole.Role.RoleName);
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Edit role";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new RoleDialogContent(ViewModel.RolesList.SelectedRole.Role.RoleName);
            dialog.PrimaryButtonClick += Dialog_EditRole;
            _ = dialog.ShowAsync();
        }

        private void Dialog_EditRole(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var dialog = sender as ContentDialog;
            var dialogContent = dialog.Content as RoleDialogContent;
            var roleName = dialogContent.RoleName;
            Debug.WriteLine(roleName);
            ViewModel.RolesList.UpdateRole(roleName);
        }
    }
}

