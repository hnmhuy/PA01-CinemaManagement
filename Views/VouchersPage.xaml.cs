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
    /// 
    public sealed partial class VouchersPage : Page
    {
        public VoucherViewModel ViewModel { get; set; }
        public VouchersPage()
        {
            this.InitializeComponent();
            var context = new DbCinemaManagementContext();
            ViewModel = new VoucherViewModel(context);
            this.DataContext = ViewModel;
        }
        private void VoucherDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = sender as DataGrid;
            Debug.WriteLine(data.SelectedIndex);
            Debug.WriteLine(data.SelectedItem);
            (data.DataContext as VoucherViewModel).SelectedVoucher = data.SelectedItem as VoucherCommand;

            //if (data != null)
            //{
            //    Debug.WriteLine(data.movie.Title);
            //}
            Debug.WriteLine(sender);
            Debug.WriteLine(e);
        }

        private void AddVoucherBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Add Voucher";
            dialog.Content = new VoucherDialogContent(null);
            dialog.PrimaryButtonText = "Add";
            dialog.SecondaryButtonText = "Cancel";
            dialog.PrimaryButtonClick += AddVoucherDialog_PrimaryButtonClick;
            dialog.XamlRoot = this.XamlRoot;
            _ = dialog.ShowAsync();
        }

        private void AddVoucherDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var dialogContent = sender.Content as VoucherDialogContent;
            ViewModel.AddVoucher(dialogContent.Voucher);
        }

        private void EditVoucher_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog(); 
            dialog.Title = "Edit Voucher";
            dialog.Content = new VoucherDialogContent(ViewModel.SelectedVoucher.Voucher);
            dialog.PrimaryButtonText = "Edit";
            dialog.SecondaryButtonText = "Cancel";
            dialog.PrimaryButtonClick += EditVoucherDialog_PrimaryButtonClick;
            dialog.XamlRoot = this.XamlRoot;
            _ = dialog.ShowAsync();
        }

        private void EditVoucherDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var dialogContent = sender.Content as VoucherDialogContent;
            ViewModel.EditVoucher(dialogContent.Voucher);
        }
    }
}
