using Caliburn.Micro;
using CinemaManagement.Models;
using LiveChartsCore.Defaults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    class AccountViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static string MALE = "Male";
        public static string FEMALE = "Female";
        public RelayCommand SaveChanges { get; set; }
        private string _gender;
        private DateTime _selectedDate;
        public BindableCollection<string> GenderList { get; set; }
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }
        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }
        public string Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AccountViewModel()
        {
            FullName = "Nguyễn Văn A";
            Gender = "Male";

            GenderList = new BindableCollection<string>
            {
                "Male",
                "Female"
            };

            SelectedDate = DateTime.Now;
            SaveChanges = new RelayCommand(Change);
        }

        public void Change(object obj)
        {
            //DbCinemaManagementContext context = new DbCinemaManagementContext();
            //if (context.Database.CanConnect())
            //{
            //    var account = context.Accounts.FirstOrDefault(a => a.AccountId == );
            //    if (account != null)
            //    {
            //        account.Fullname = FullName;
            //        account.Gender = Gender;
            //        context.SaveChanges();
            //    }
            //    context.SaveChanges();
            //}
            Debug.WriteLine("Change");
        }
    
}
}
