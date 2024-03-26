using CinemaManagement.Models;
using CinemaManagement.WindowViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
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
        private bool _isChecked;

        public Voucher Voucher { get; set; }
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                OnPropertyChanged(nameof(IsAvailable));
            }
        }

        public bool IsChecked
        {
            get => this._isChecked;
            set
            {
                this._isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
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
        public static string VIPTICKET = "VIP Ticket";
        public static string NORMAL_TICKET = "Normal Ticket";
        public static int BOOKING_SUCCESS = 1;
        public static int BOOKING_FAIL = 0;

       
        // INotifyPropertyChanged Interface implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private int movieId;
        private DbCinemaManagementContext db = new DbCinemaManagementContext(); 
        public ObservableCollection<Ticket> ticketList { get; set; }
        public ObservableCollection<Ticket> selectedTickets { get; set; } = new ObservableCollection<Ticket>();
        public ObservableCollection<Voucher> vouchers { get; set; }
        public ObservableCollection<CheckBoxVoucher> checkBoxVouchers { get; set; } = new ObservableCollection<CheckBoxVoucher>();
        public ObservableCollection<BillRow> billRowList { get; set; } = new ObservableCollection<BillRow>();
        private Movie movie { get; set; }
        public Movie CurrMovie
        {
            get => movie;
            set
            {
                movie = value;
                OnPropertyChanged(nameof(CurrMovie));
            }
        }
        private ShowTime displayingShowtime { get; set; }
        public ShowTime DisplayingShowTime
        {
            get => displayingShowtime;
            set
            {
                displayingShowtime = value;
                OnPropertyChanged(nameof(DisplayingShowTime));
            }
        }
        public List<ShowTime> showTimes { get; set; }
        public ObservableCollection<string> rowList { get; set; }
        private int seatMapWidth { get; set; }
        private int seatMapHeight { get; set; }
        public bool allowDialog { get; set; }
        private bool haveShowtime { get; set; }
        public bool HaveShowtime
        {
            get => haveShowtime;
            set
            {
                haveShowtime = value;
                OnPropertyChanged(nameof(HaveShowtime));
            }
        }
        public int SeatMapWidth
        {
            get => seatMapWidth;
            set
            {
                seatMapWidth = value;
                OnPropertyChanged(nameof(SeatMapWidth));
            }
        }
        public int SeatMapHeight
        {
            get => seatMapHeight;
            set
            {
                seatMapHeight = value;
                OnPropertyChanged(nameof(SeatMapHeight));
            }
        }
        private int maxCol { get; set; }
        public int MaxCol
        {
            get => maxCol;
            set
            {
                maxCol = value;
                OnPropertyChanged(nameof(MaxCol));
            }
        }
        public ObservableCollection<GroupContributor> contributorList { get; set; }
        public ObservableCollection<string> showHours { get; set; }  = new ObservableCollection<string>();
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
        private DateTime _selectedDate;
        public DateTime selectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                Debug.WriteLine("Selected date: " + value.ToString());
                FindShowTimeByDate();
                OnPropertyChanged(nameof(selectedDate));
            }
        }
        private int selectedShowTimeIndex {  get; set; }
        public int SelectedShowTimeIndex
        {
            get => selectedShowTimeIndex;
            set
            {
                selectedShowTimeIndex = value;
                Debug.WriteLine("Showtime index: " + value);
                SelectShowTime(value);
                OnPropertyChanged(nameof(selectedShowTimeIndex));
            }
        }
        private bool isAuthenticated { get; set; }
        public bool IsAuthenticated
        {
            get => isAuthenticated;
            set
            {
                isAuthenticated = value;
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }

        public RelayCommand BookTicket { get; set; }
        public RelayCommand Authenticate { get; set; }

        private int _notifyCode;
        public int NotifyCode
        {
            get => _notifyCode;
            set
            {
                _notifyCode = value;
                OnPropertyChanged(nameof(NotifyCode));
            }
        }

        private bool isDataLoaded = false;

        public string NotifyMessage { get; set; }
        
        public MovieDetailViewModel(int id)
        {
            this.movieId = id;
            LoadData();
            showHours = new ObservableCollection<string>();
            allowDialog = true;
            haveShowtime = false;
            var session = AuthenticationControl.RestoreSession();
            IsAuthenticated = session.Item1 && session.Item2 != -1;
            Authenticate = new RelayCommand(OnAuthenticate);
            BookTicket = new RelayCommand(OnBookTicket);
            if (IsAuthenticated)
            {
                AddBirthDateVoucher();
            }
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
                checkBoxVouchers.Add(new CheckBoxVoucher() { Voucher = voucher, IsAvailable = true, IsChecked = false });
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
                rowList = new ObservableCollection<string>();
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
        public void LoadData()
        {            

            CurrMovie = (Movie)db.Movies.Where(m => m.MovieId == movieId).FirstOrDefault();
            CurrMovie.AgeCertificate = db.Movies.Where(m => m.MovieId == movieId).Select(m => m.AgeCertificate).FirstOrDefault();
            genreList = db.Movies.Where(m => m.MovieId == movieId).SelectMany(m => m.Genres).ToList();
            LoadContributors();
            LoadVouchers();
            isDataLoaded = true;
        }
        private void LoadVouchers()
        {
            var temp = db.Vouchers.Where(v => !v.IsExpired && v.VoucherAmount > 0 && !v.VoucherCode.Contains("BIRTHDATE")).ToList();
            if (this.vouchers == null)
            {
                this.vouchers = new ObservableCollection<Voucher>();
            }
            else
            {
                this.vouchers.Clear();
            }
            foreach (Voucher voucher in temp)
            {
                this.vouchers.Add(voucher);
            }
            GenerateCheckboxVouchers();
        }
        private void LoadContributors()
        {
            var contributorsInMovie = db.Movies
                .Include(m => m.Contributors)
                .ThenInclude(c => c.Person)
                .Include(m => m.Contributors)
                .ThenInclude(c => c.Role)
                .Where(m => m.MovieId == movieId).FirstOrDefault().Contributors;

            var groupContributors = contributorsInMovie.GroupBy(c => c.Role.RoleName);
            if (this.contributorList != null) this.contributorList.Clear(); else this.contributorList = new ObservableCollection<GroupContributor>();
            foreach (var group in groupContributors)
            {
                List<ContributorAndSeparator> contributors = new List<ContributorAndSeparator>();
                foreach (var contributor in group)
                {
                    if (contributor == group.Last())
                    {
                        contributors.Add(new ContributorAndSeparator(contributor.Person.Fullname, false));
                    }
                    else
                        contributors.Add(new ContributorAndSeparator(contributor.Person.Fullname));
                }
                this.contributorList.Add(new GroupContributor(group.Key, contributors));
            }
            
        }
        private void FindShowTimeByDate()
        {
            // Extract the date from selectedDate
            if (showTimes == null) showTimes = new List<ShowTime>(); else showTimes.Clear();
            showTimes = db.ShowTimes.Where(s => s.MovieId == movieId && s.ShowDate.Date == selectedDate.Date).ToList();

            showHours.Clear();
            foreach (ShowTime showTime in showTimes)
            {
                // Convert to 24 hour format and add to showHour
                if (showTime.ShowDate < DateTime.Now) continue;
                showHours.Add(showTime.ShowDate.ToString("HH:mm"));
                Debug.WriteLine("Showtime: " + showTime.ShowDate.ToString() + " - " + showTime.MovieId);
            }

            if (showHours.Count > 0)
            {
                SelectedShowTimeIndex = 0;
                HaveShowtime = true;
            }
            else
            {
                HaveShowtime = false;
            }
        
        }    
        private void CalculateSeatMapSize()
        {
            SeatMapWidth = (int)displayingShowtime.MaxCol * 52 + ((int)displayingShowtime.MaxCol - 1) * 12;
            SeatMapHeight = (int)displayingShowtime.MaxRow * 52 + ((int)displayingShowtime.MaxRow - 1) * 12;
        }
        private void LoadTicket()
        {
            if (displayingShowtime != null)
            {
                var tickets = db.Tickets.Where(t => t.ShowTimeId == displayingShowtime.ShowTimeId)
                    .OrderBy(t => t.Row).ThenBy(t => t.Col).ToList();
                if (this.ticketList != null) ticketList.Clear();
                else this.ticketList = new ObservableCollection<Ticket>();
                foreach (Ticket t in tickets)
                {
                    //Debug.WriteLine(t.TicketId);
                    ticketList.Add(t);
                }
            }
        }
        public void SelectShowTime(int index)
        {
            if (!isDataLoaded || this.showTimes.Count <= 0 || index < 0 || index >= showTimes.Count) return;
            allowDialog = false;
            selectedTickets.Clear();
            billRowList.Clear();
            DisplayingShowTime = showTimes[index];
            CalculateSeatMapSize();
            //GenerateTicketList();
            LoadTicket();
            this.MaxCol = DisplayingShowTime.MaxCol;
            ConvertRowListToStringList(DisplayingShowTime.MaxRow);
            RemoveAllVoucher();
            CalculateTotalBill();
            allowDialog = true;
        }
        public void RemoveAllVoucher()
        {
            foreach (var item in checkBoxVouchers)
            {
                item.IsChecked = false;
            }
        }
        // Commands
        public void AddTicket(Ticket ticket)
        {
            selectedTickets.Add(ticket);
            var billRow = billRowList.FirstOrDefault(x => x.content == ((bool)ticket.IsVip ? VIPTICKET : NORMAL_TICKET));
            if (billRow == null)
            {
                billRowList.Add(new BillRow(1, (double)ticket.Price, (bool)ticket.IsVip ? VIPTICKET : NORMAL_TICKET));
            } else
            {
                billRow.AddTicket((double)ticket.Price);
            }
            _totalTicketPrice += (double)ticket.Price;
            CalculateTotalBill();
        }
        public void RemoveTicket(Ticket ticket)
        {
            if (ticket == null) return;
            selectedTickets.Remove(ticket);
            var billRow = billRowList.FirstOrDefault(x => x.content == ((bool)ticket.IsVip ? VIPTICKET : NORMAL_TICKET));
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

        public void OnAuthenticate(object obj)
        {
            AuthenticateWindow authenticateWindow = new AuthenticateWindow();
            authenticateWindow.Activate();
            authenticateWindow.Closed += AuthenticateWindow_Closed;
        }

        private void AuthenticateWindow_Closed(object sender, WindowEventArgs args)
        {
            var returnValue = (sender as AuthenticateWindow).returnValue;   
            IsAuthenticated = returnValue.Item1;

            // If this month is the user's birthday, add a birthday voucher
            AddBirthDateVoucher();
        }

        private void AddBirthDateVoucher()
        {
            var returnValue = AuthenticationControl.RestoreSession();
            var userData = db.Accounts.Where(a => a.AccountId == returnValue.Item2).FirstOrDefault();
            if (DateTime.Now.Month == userData.Dob.Month)
            {
                var temp = db.Vouchers.Where(v => v.VoucherCode.Equals("BIRTHDATE" + DateTime.Now.Month)).FirstOrDefault();
                if (temp == null)
                {
                    // Create a new voucher
                    var voucher = new Voucher()
                    {
                        DiscountAmount = 50,
                        IsExpired = false,
                        IsPercentageDiscount = true,
                        RequirementAmount = 0,
                        VoucherAmount = 1,
                        VoucherCode = "BIRTHDATE" + DateTime.Now.Month
                    };
                    db.Vouchers.Add(voucher);
                    db.SaveChanges();
                    vouchers.Add(voucher);
                    temp = voucher;
                } else if (temp.VoucherAmount == 0)
                {
                    temp.VoucherAmount = 1;
                    db.Vouchers.Update(temp);
                    db.SaveChanges();
                }
                checkBoxVouchers.Add(new CheckBoxVoucher() { Voucher = temp, IsAvailable = true, IsChecked = false });

            }
        }

        public bool CanBookTicket(object obj)
        {
            Debug.WriteLine(IsAuthenticated + " - " + this.selectedTickets.Count);
            return IsAuthenticated && this.selectedTickets.Count > 0;
        }

        public void OnBookTicket(object obj)
        {
            if (db.Database.CanConnect())
            {
                int tempTotal = 0;
                Bill bill = new Bill() { AccountId = AuthenticationControl.RestoreSession().Item2, BookingTime = DateTime.Now, Total = 0};
                foreach(Ticket ticket in selectedTickets)
                {
                    var ticketEntity = db.Tickets.Where(t => t.TicketId == ticket.TicketId).FirstOrDefault();
                    ticketEntity.IsAvailable = false;
                    ticketEntity.Bill = bill;
                    db.Update(ticketEntity);                    
                    tempTotal += (int)ticket.Price;
                }
                foreach(BillRow billRow in billRowList)
                {
                    if (billRow.Voucher != null)
                    {
                        db.Vouchers.Where(v => v.VoucherId == billRow.Voucher.VoucherId).FirstOrDefault().VoucherAmount -= 1;
                        db.BillVouchers.Add(new BillVoucher() { Bill = bill, Voucher = billRow.Voucher, AppliedTime = DateTime.Now });
                        tempTotal += (int)billRow.price;
                    }
                }
                bill.Total = tempTotal;

                db.Bills.Add(bill);
                try
                {
                    db.SaveChanges();
                    SelectShowTime(SelectedShowTimeIndex);
                    NotifyCode = BOOKING_SUCCESS;
                    NotifyMessage = "Booking successfully";
                } catch (Exception e)
                {
                    NotifyCode = BOOKING_FAIL;
                    NotifyMessage = "Booking failed";
                    Debug.WriteLine(e.Message);
                }
            }
        }

    }

    public class TicketTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
           bool isViP = (bool)value;
            return isViP ? "VIP" : "Normal";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
