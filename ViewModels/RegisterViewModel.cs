using CinemaManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        // Override the OnPropertyChanged method to raise the PropertyChanged event and
        // to set the property value.
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static string MALE = "Male";
        public static string FEMALE = "Female";
        public RelayCommand RegisterCommand { get; set; }
        private string _userName;
        private string _password;
        private DateTime _dob;
        private string _gender;
        public DateTime minDate = new DateTime(1900, 1, 1);
        // Max dob is for people who are 13 years old from now
        public DateTime maxDate = DateTime.Now.AddYears(-13);

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public DateTime Dob
        {
            get { return _dob; }
            set
            {
                _dob = value;
                OnPropertyChanged(nameof(Dob));
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

        public RegisterViewModel()
        {
            UserName = "";
            Password = "";
            Dob = maxDate;
            RegisterCommand = new RelayCommand(Register, Validate); 
        }

        public bool Validate(object obj)
        {
            //if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            //{
            //    return false;
            //}
            return true;
        }

        // Hash the password before sending it to the server.
        public string HashPassword()
        {
            return BCrypt.Net.BCrypt.HashPassword(Password);
        }

        public void Register(object obj)
        {
            DbCinemaManagementContext context = new DbCinemaManagementContext();
            if (context.Database.CanConnect())
            {
                var account = context.Accounts.Where(a => a.Username == UserName).FirstOrDefault();
                if (account != null)
                {
                    return;
                }
                Account newAccount = new Account()
                {
                    Username = UserName,
                    Password = HashPassword(),
                    Dob = Dob,
                    Gender = Gender,
                    IsAdmin = false,
                };
                context.Accounts.Add(newAccount);
                context.SaveChanges();
            }
        }
    }
}
