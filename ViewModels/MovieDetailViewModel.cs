using CinemaManagement.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CinemaManagement.ViewModels
{
    public struct ContributorAndSeparator
    {
        public string Contributor { get; set; }
        public bool isSeparatorVisible { get; set; }

        public ContributorAndSeparator(string contributor, bool isSeparatorVisible = true)
        {
            Contributor = contributor;
            this.isSeparatorVisible = isSeparatorVisible;
        }
    }

    public struct GroupContributor
    {
        public string GroupName { get; set; }
        public List<ContributorAndSeparator> Contributors { get; set; }
        public GroupContributor(string groupName, List<ContributorAndSeparator> contributors)
        {
            GroupName = groupName;
            Contributors = contributors;
        }
    }

    public class BillRow : INotifyPropertyChanged
    {
        private int _amount;
        private double _price;
        public int amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged("amount");
            }
        }
        public double price
        {
            get => _price; set
            {
                _price = value;
                OnPropertyChanged("price");
            }
        }
        public string content { get; set; }

        public Voucher? Voucher { get; set; }
        public BillRow(int amount, double price, string content, Voucher voucher = null)
        {
            this.amount = amount;
            this.price = price;
            this.content = content;
            Voucher = voucher;

        }

        public void AddTicket(double price)
        {
            amount++;
            this.price += price;
        }

        public void RemoveTicket(double price)
        {
            if (amount > 0)
            {
                amount--;
                this.price -= price;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CheckBoxVoucher : INotifyPropertyChanged
    {
        private bool _isAvailable;
        public Voucher Voucher { get; set; }
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                OnPropertyChanged("IsAvailable");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MovieDetailViewModel : INotifyPropertyChanged
    {
        public const string VIP_TICKET = "VIP Ticket";
        public const string NORMAL_TICKET = "Normal Ticket";

        // INotifyPropertyChanged Interface implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICollection<Ticket> ticketList { get; set; }
        public ObservableCollection<Ticket> selectedTickets { get; set; }
        public ObservableCollection<Voucher> vouchers { get; set; }
        public ObservableCollection<CheckBoxVoucher> checkBoxVouchers { get; set; }
        public ObservableCollection<BillRow> billRowList { get; set; }
        public Movie movie { get; set; }
        public ShowTime ShowTime { get; set; }
        public List<string> rowList { get; set; }
        public int seatMapWidth { get; set; }
        public int seatMapHeight { get; set; }
        public List<GroupContributor> contributorList { get; set; }
        public List<Genre> genreList { get; set; }
        private double _currentTotalBill;
        public double currentTotalBill
        {
            get => _currentTotalBill;
            set
            {
                _currentTotalBill = value;
                OnPropertyChanged("currentTotalBill");
            }
        }
        private double _totalTicketPrice;
        public MovieDetailViewModel()
        {
            selectedTickets = new ObservableCollection<Ticket>();
            vouchers = new ObservableCollection<Voucher>();
            billRowList = new ObservableCollection<BillRow>();
            checkBoxVouchers = new ObservableCollection<CheckBoxVoucher>();

            ShowTime = new ShowTime()
            {
                ShowTimeId = 1,
                ShowDate = DateTime.Now,
                MaxRow = 8,
                MaxCol = 18,
                MovieId = 1,
                Movie = movie,
            };

            GenerateTicketList();
            ConvertRowListToStringList((int)ShowTime.MaxRow);

            genreList = new List<Genre>();
            genreList.Add(new Genre() { GenreId = 1, GenreName = "Action" });
            genreList.Add(new Genre() { GenreId = 2, GenreName = "Adventure" });
            genreList.Add(new Genre() { GenreId = 3, GenreName = "Comedy" });
            AgeCertificate ageCertificate = new AgeCertificate { AgeCertificateId = 1, DisplayContent = "C13", RequireAge = 13, ForegroundColor = "Orange", BackgroundColor = "Transparent" };
            GenerateContributorList();
            this.movie = new Movie
            {
                Title = "Dune Part Two",
                Duration = 120,
                PublishYear = 2022,
                Imdbrating = 7.8,
                AgeCertificateId = 2,
                AgeCertificate = ageCertificate,
                PosterPath = "/Assets/Images/Poster/dune2.jpg",
                Description = "Dune 2 is the sequel to Dune (2021)1. It is the second of a two-part adaptation of the 1965 novel Dune by Frank Herbert1. The movie follows Paul Atreides as he unites with the Fremen people of the desert planet Arrakis to wage war against House Harkonnen1",
                TrailerPath = "/Assets/Videos/dune2.mp4"
            };

            GenerateVoucherList();
            GenerateCheckboxVouchers(); 
        }

        // Utils
        public void GenerateCheckboxVouchers()
        {
            if (checkBoxVouchers == null)
            {
                checkBoxVouchers = new ObservableCollection<CheckBoxVoucher>();
            }
            else
            {
                checkBoxVouchers.Clear();
            }
            foreach(Voucher voucher in vouchers)
            {
                checkBoxVouchers.Add(new CheckBoxVoucher() { Voucher = voucher, IsAvailable = true });
            }

        }
        public string ConvertRowToString(int row)
        {
            return ((char)('A' + row)).ToString();
        }
        public void ConvertRowListToStringList(int maxRow)
        {
            if (rowList == null)
            {
                rowList = new List<string>();
            }
            else
            {
                rowList.Clear();
            }

            for (int i = 0; i < maxRow; i++)
            {
                rowList.Add(ConvertRowToString(i));
            }
        }

        // Testing UI function
        public void GenerateTicketList()
        {
            // Generate ticket list
            if (ticketList == null)
            {
                ticketList = new List<Ticket>();
            }
            else
            {
                ticketList.Clear();
            }

            for (int i = 0; i < ShowTime.MaxRow; i++)
            {
                for (int j = 0; j < ShowTime.MaxCol; j++)
                {
                    ticketList.Add(new Ticket()
                    {
                        IsAvailable = true,
                        IsVip = i % 2 == 0,
                        Price = i % 2 == 0 ? 90000 : 60000,
                        TicketId = 001,
                        Col = j,
                        Row = ConvertRowToString(i),
                    });
                }
            }

            seatMapWidth = (int)ShowTime.MaxCol * 50 + ((int)ShowTime.MaxCol - 1) * 12;
            seatMapHeight = (int)ShowTime.MaxRow * 50 + ((int)ShowTime.MaxRow - 1) * 12;

            // Randomly set some tickets to unavailable
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                int randomRow = random.Next(0, (int)ShowTime.MaxRow);
                int randomCol = random.Next(0, (int)ShowTime.MaxCol);
                ticketList.ElementAt(randomRow * (int)ShowTime.MaxCol + randomCol).IsAvailable = false;
            }
        }
        public void GenerateContributorList()
        {
            if (contributorList == null)
            {
                contributorList = new List<GroupContributor>();
            }
            else
            {
                contributorList.Clear();
            }
            // Director 
            List<ContributorAndSeparator> directorList = new List<ContributorAndSeparator>();
            directorList.Add(new ContributorAndSeparator("Director 1"));
            directorList.Add(new ContributorAndSeparator("Director 2", false));
            contributorList.Add(new GroupContributor("Director", directorList));
            // Writers
            List<ContributorAndSeparator> writerList = new List<ContributorAndSeparator>();
            writerList.Add(new ContributorAndSeparator("Writer 1"));
            writerList.Add(new ContributorAndSeparator("Writer 2", false));
            contributorList.Add(new GroupContributor("Writer", writerList));
            // Stars
            List<ContributorAndSeparator> starList = new List<ContributorAndSeparator>();
            starList.Add(new ContributorAndSeparator("Star 1"));
            starList.Add(new ContributorAndSeparator("Star 2", false));
            contributorList.Add(new GroupContributor("Star", starList));
        }
        public void GenerateVoucherList()
        {
            vouchers.Add(new Voucher() { VoucherId = 1, VoucherCode = "VOUCHER1", VoucherAmount = 10, DiscountAmount = 12000, IsPercentageDiscount = false, IsExpired = false, RequirementAmount=100000 });
            vouchers.Add(new Voucher() { VoucherId = 2, VoucherCode = "VOUCHER2", VoucherAmount = 12, DiscountAmount = 15, IsPercentageDiscount = true, IsExpired = false, RequirementAmount= 100000 });
            vouchers.Add(new Voucher() { VoucherId = 3, VoucherCode = "VOUCHER3", VoucherAmount = 15, DiscountAmount = 20, IsPercentageDiscount = true, IsExpired = false, RequirementAmount= 100000 });
        }

        // Commands
        public void AddTicket(Ticket ticket)
        {
            selectedTickets.Add(ticket);
            var billRow = billRowList.FirstOrDefault(x => x.content == ((bool)ticket.IsVip ? VIP_TICKET : NORMAL_TICKET));
            if (billRow == null)
            {
                billRowList.Add(new BillRow(1, (double)ticket.Price, (bool)ticket.IsVip ? VIP_TICKET : NORMAL_TICKET));
            } else
            {
                billRow.AddTicket((double)ticket.Price);
            }
            _totalTicketPrice += (double)ticket.Price;
            CalculateTotalBill();
        }
        public void RemoveTicket(Ticket ticket)
        {
            selectedTickets.Remove(ticket);
            var billRow = billRowList.FirstOrDefault(x => x.content == ((bool)ticket.IsVip ? VIP_TICKET : NORMAL_TICKET));
            if (billRow != null)
            {
                billRow.RemoveTicket((double)ticket.Price);
                if (billRow.amount == 0)
                {
                    billRowList.Remove(billRow);
                }
            }
            _totalTicketPrice -= (double)ticket.Price;
            CalculateTotalBill();
        }

        private (bool, string) CanAddVoucher(Voucher v)
        {
            if (v == null) return (false, "Null voucher");
            if ((bool)v.IsExpired) return (false, "Expired voucher");
            if (this._totalTicketPrice < v.RequirementAmount) return (false, "Total bill is not enough. This requires at lease " + ((double)v.RequirementAmount).ToString("C0", new System.Globalization.CultureInfo("vi-VN")));
            return (true, "Added successfully");
        }

        public (bool, string) AddVoucher(CheckBoxVoucher v)
        {
            var (canAdd, message) = CanAddVoucher(v.Voucher);
            if (canAdd)
            {
                // Add a voucher to the bill
                var billRow = billRowList.FirstOrDefault(x => x.Voucher == v.Voucher);
                if (billRow !=null)
                {
                    billRow.price = CalculateVoucherDiscount(v.Voucher);
                } else
                {
                    billRowList.Add(new BillRow(1, CalculateVoucherDiscount(v.Voucher), v.Voucher.VoucherCode, v.Voucher));
                }
                CalculateTotalBill();
                return (true, "Added successfully");
            }
            return (false, message);
        }
        public (bool, string) RemoveVoucher(CheckBoxVoucher v)
        {
            var billRow = billRowList.FirstOrDefault(x => x.Voucher == v.Voucher);
            if (billRow != null)
            {
                billRowList.Remove(billRow);
                CalculateTotalBill();
                return (true, "Removed successfully");
            } else
            {
                return (false, "Voucher not found");
            }
        }

        private double CalculateVoucherDiscount(Voucher voucher)
        {
            if (voucher == null) return 0;
            if ((bool)voucher.IsPercentageDiscount)
            {
                return (double)voucher.DiscountAmount / 100 * _totalTicketPrice * -1;
            }
            return (double)voucher.DiscountAmount * -1;
        }

        public void UpdateVouchersValue()
        {
            foreach (BillRow voucherRow in billRowList)
            {
                if (voucherRow.Voucher != null)
                {
                    var checkboxVoucher = checkBoxVouchers.FirstOrDefault(x => x.Voucher == voucherRow.Voucher);
                    if ((double)voucherRow.Voucher.RequirementAmount > _totalTicketPrice)
                    {
                        // Find the checkbox voucher and set it to unavailable
                        if (checkboxVoucher != null)
                        {
                            checkboxVoucher.IsAvailable = false;
                        }
                        voucherRow.price = 0;
                    } else                 
                    {
                        if (checkboxVoucher != null)
                        {
                            checkboxVoucher.IsAvailable = true;
                        }
                        voucherRow.price = CalculateVoucherDiscount(voucherRow.Voucher);
                    }
                }
            }
        }

        public void CalculateTotalBill()
        {
            UpdateVouchersValue();
            double total = 0;
            foreach (BillRow billRow in billRowList)
            {
                total += billRow.price;
            }
            if(total < 0) total = 0;    
            currentTotalBill = total;
        }
    }
    
}
