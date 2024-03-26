using CinemaManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{

    public class TicketCount
    {
        public int numberOfTickets { get; set; }
        public string ticketType { get; set; }
        public double price { get; set; }

        public double totalPrice { get; set; } = 0;

        public TicketCount(int numberOfTickets, string ticketType, double price)
        {
            this.numberOfTickets = numberOfTickets;
            this.ticketType = ticketType;
            this.price = price;
            this.totalPrice = price * numberOfTickets;
        }
    }

    public class MyTicketDisplay
    {
        public ShowTime showTime { get; set; }
        public List<Ticket> ticketsList { get; set; }
        public Bill Bill { get; set; }
        public List<TicketCount> ticketCounts { get; set; }
        public List<Voucher> Voucher { get; set; }
        public double discount { get; set; } = 0;

        public MyTicketDisplay(int showtimeId, Bill bill)
        {
            LoadShowtime(showtimeId);
            this.Bill = bill;
            ProcessBill();
        }

        private void LoadShowtime(int showtimeId)
        {
            using (var db = new DbCinemaManagementContext())
            {
                this.showTime = db.ShowTimes.Include(st => st.Movie).Where(st => st.ShowTimeId == showtimeId).FirstOrDefault();
            }
        }

        public void ProcessBill()
        {
            this.ticketsList = this.Bill.Tickets.ToList();
            this.ticketCounts = new List<TicketCount>();
            foreach (var ticket in this.ticketsList)
            {
                string type = ticket.IsVip ? "VIP" : "Normal";
                // Find the ticket count with the same type
                var ticketCount = this.ticketCounts.Find(tc => tc.ticketType == type);
                if (ticketCount == null)
                {
                    ticketCount = new TicketCount(1, type, ticket.Price);
                    this.ticketCounts.Add(ticketCount);
                }
                else
                {
                    ticketCount.numberOfTickets++;
                    ticketCount.totalPrice += ticket.Price;
                }
            }
            var db = new DbCinemaManagementContext();   
            var temp = db.BillVouchers.Include(bv => bv.Voucher).Where(bv => bv.BillId == this.Bill.BillId).Select(bv => bv.Voucher).ToList();
            this.Voucher = temp;
            this.discount = this.Bill.Total - this.ticketCounts.Sum(tc => tc.totalPrice);
        }

    }

    public class MyTicketsViewModel : INotifyPropertyChanged
    {
        public List<MyTicketDisplay> MyTickets { get; set; }

        private bool _isAuthenticated = false;  
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set
            {
                _isAuthenticated = value;
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }

        private bool _isEmpty = false;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set
            {
                _isEmpty = value;
                OnPropertyChanged(nameof(IsEmpty));
            }
        }

        public MyTicketsViewModel()
        {
            var authenticateData = AuthenticationControl.RestoreSession();
            if (authenticateData.Item1 && authenticateData.Item2 != -1)
            {
                LoadBill(authenticateData.Item2);
                IsEmpty = MyTickets.Count == 0;
                IsAuthenticated = true;
            } else
            {
                IsEmpty = false;
                IsAuthenticated = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void LoadBill(int accountId)
        {
            var db = new DbCinemaManagementContext();
            var bills = db.Bills.Include(b => b.Tickets).Include(b => b.BillVouchers).Where(b => b.AccountId == accountId).ToList();
            this.MyTickets = new List<MyTicketDisplay>();
            foreach (var bill in bills)
            {
                foreach (var ticket in bill.Tickets)
                {
                    var showtimeId = ticket.ShowTimeId;
                    var myTicket = this.MyTickets.Find(mt => mt.showTime.ShowTimeId == showtimeId);
                    if (myTicket == null && showtimeId != null)
                    {
                        myTicket = new MyTicketDisplay((int)showtimeId, bill);
                        this.MyTickets.Add(myTicket);
                    }
                }
            }
        }

    }
    public class VoucherListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<Voucher> vouchers = value as List<Voucher>;
            if (vouchers == null)
            {
                return "No voucher";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var voucher in vouchers)
            {
                if (voucher != null)
                {
                    sb.Append(voucher.VoucherCode);
                    sb.Append(", ");
                }
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
