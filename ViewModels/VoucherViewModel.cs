using CinemaManagement.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    public class VoucherViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Voucher> VouchersList { get; set; }
        public Voucher SelectedVoucher { get; set; }
        public VoucherViewModel()
        {
            VouchersList = GenerateSampleData();
        }

        private ObservableCollection<Voucher> GenerateSampleData()
        {
            // Generate sample data for G
            ObservableCollection<Voucher> res = new ObservableCollection<Voucher>();
            var Voucher1 = new Voucher
            {
                VoucherCode = "ABCDEF1523",
                IsExpired = false,
                VoucherAmount = 30,
                VoucherId = 1
            };
            res.Add(Voucher1);

            var Voucher2 = new Voucher
            {
                VoucherCode = "ABCD615523",
                IsExpired = false,
                VoucherAmount = 30,
                VoucherId = 2
            };

            // Add Voucher 2 to the result list
            res.Add(Voucher2);

            var Voucher3 = new Voucher
            {
                VoucherCode = "AB5261523",
                IsExpired = true,
                VoucherAmount = 50,
                VoucherId = 3
            };

            // Add Voucher 3 to the result list
            res.Add(Voucher3);
            var Voucher4 = new Voucher
            {
                VoucherCode = "AB5261523",
                IsExpired = true,
                VoucherAmount = 50,
                VoucherId = 4
            };

            // Add Voucher 3 to the result list
            res.Add(Voucher4);
            var Voucher5 = new Voucher
            {
                VoucherCode = "AB5261523",
                IsExpired = true,
                VoucherAmount = 50,
                VoucherId = 5
            };

            // Add Voucher 3 to the result list
            res.Add(Voucher5);


            return res;
        }




    }

    public class TotalVouchersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<Voucher> vouchers)
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
            if (value is Voucher vouchers)
            {
               if (!vouchers?.IsExpired ?? false)
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
            if (value is Voucher voucher)
            {
                if (!voucher?.IsExpired ?? false)
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
            if (value is Voucher voucher)
            {
                if (!voucher?.IsExpired ?? false)
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

}
