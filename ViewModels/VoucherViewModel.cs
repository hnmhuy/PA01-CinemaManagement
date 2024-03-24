using CinemaManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{

    public class VoucherCommand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand DeleteCommand { get; set; }
        public Voucher Voucher { get; set; }

        public VoucherCommand(Voucher _Voucher, RelayCommand _deleteCommand)
        {
            this.Voucher = _Voucher;
            this.DeleteCommand = _deleteCommand;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
    public class VoucherViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly DbCinemaManagementContext _context;
        public ObservableCollection<VoucherCommand> VouchersList { get; set; }
        public VoucherCommand SelectedVoucher { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        public VoucherViewModel(DbCinemaManagementContext context)
        {
            _context = context;
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            VouchersList = GenerateSampleData(DeleteCommand);
            SelectedVoucher = VouchersList[0];

        }

        private async Task DeleteVoucherAsync(Voucher voucher)
        {
            try
            {
                _context.BillVouchers.RemoveRange(voucher.BillVouchers);
                _context.Vouchers.Remove(voucher);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting genre: {ex.Message}");
            }
        }

        private bool CanDelete(object parameter)
        {

            return SelectedVoucher!= null;
        }

        private void OnDelete(object obj)
        {
            // Print a debug message to indicate that the OnDelete method is being called
            Debug.WriteLine("OnDelete method called.");

            // Check if SelectedGenre is correctly set
            if (SelectedVoucher != null)
            {
                Debug.WriteLine($"Deleting Genre: {SelectedVoucher.Voucher.VoucherCode}");

                DeleteVoucherAsync(SelectedVoucher.Voucher); // Call the method to delete from the database

                VouchersList.Remove(SelectedVoucher);
            }
            else
            {
                // Print a debug message if SelectedGenre is null
                Debug.WriteLine("SelectedGenre is null. Cannot delete.");
            }

        }

        public VoucherViewModel()
        {
            //VouchersList = GenerateSampleData();
        }

        private ObservableCollection<VoucherCommand> GenerateSampleData(RelayCommand DeleteCommand)
        {
            // Generate sample data for G
            ObservableCollection<VoucherCommand> res = new ObservableCollection<VoucherCommand>();
            var vouchers = _context.Vouchers
                .Include(m => m.BillVouchers)
                .ToList();

            foreach (var voucher in vouchers)
            {
                res.Add(new VoucherCommand(voucher, DeleteCommand));
            }
            return res;
            //var Voucher1 = new Voucher
            //{
            //    VoucherCode = "ABCDEF1523",
            //    IsExpired = false,
            //    VoucherAmount = 30,
            //    VoucherId = 1
            //};
            //res.Add(Voucher1);

            //var Voucher2 = new Voucher
            //{
            //    VoucherCode = "ABCD615523",
            //    IsExpired = false,
            //    VoucherAmount = 30,
            //    VoucherId = 2
            //};

            //// Add Voucher 2 to the result list
            //res.Add(Voucher2);

            //var Voucher3 = new Voucher
            //{
            //    VoucherCode = "AB5261523",
            //    IsExpired = true,
            //    VoucherAmount = 50,
            //    VoucherId = 3
            //};

            //// Add Voucher 3 to the result list
            //res.Add(Voucher3);
            //var Voucher4 = new Voucher
            //{
            //    VoucherCode = "AB5261523",
            //    IsExpired = true,
            //    VoucherAmount = 50,
            //    VoucherId = 4
            //};

            //// Add Voucher 3 to the result list
            //res.Add(Voucher4);
            //var Voucher5 = new Voucher
            //{
            //    VoucherCode = "AB5261523",
            //    IsExpired = true,
            //    VoucherAmount = 50,
            //    VoucherId = 5
            //};

            //// Add Voucher 3 to the result list
            //res.Add(Voucher5);


            //return res;
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class TotalVouchersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<VoucherCommand> vouchers)
            {
                int total = 0;
                total = vouchers.Count;
                return total;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class StatusChangeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is VoucherCommand voucher)
            {
               if (!voucher.Voucher?.IsExpired ?? false)
                {
                    return "Active";
                }
                else
                {
                    return "Expired";

                }
                
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusChangeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is VoucherCommand voucher)
            {
                if (!voucher.Voucher?.IsExpired ?? false)
                    return "#4CBB17"; // Active color
                else
                    return "#FF0000"; // Expired color
            }
            return "#000000"; // Default color if not a Voucher or other error
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusChangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is VoucherCommand voucher)
            {
                if (!voucher.Voucher?.IsExpired ?? false)
                {
                    return new TextWithColor { Status = "Active", Color = "#4CBB17" };
                }
                else
                {
                    return new TextWithColor { Status = "Expired", Color = "#FF0000" };
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public class TextWithColor
        {
            public string Status { get; set; }
            public string Color { get; set; }
        }
    }

    public class TotalUseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is VoucherCommand voucherCommand)
            {
                if (voucherCommand.Voucher != null && voucherCommand.Voucher.BillVouchers != null)
                {
                    int totalUse = voucherCommand.Voucher.BillVouchers.Count();
                    return totalUse;
                }
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
