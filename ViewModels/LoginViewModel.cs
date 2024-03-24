using CinemaManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        // Override the OnPropertyChanged method to raise the PropertyChanged event and
        // to set the property value.
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _userName;
        private string _password;
        public (bool, Account, string) value;

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

        public LoginViewModel()
        {
            UserName = "";
            Password = "";
        }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return false;
            }
            return true;
        }

        // Hash the password before sending it to the server.
        public string HashPassword()
        {
            return BCrypt.Net.BCrypt.HashPassword(Password);
        }

        public void Login()
        {
            Debug.WriteLine("Login function run");
            if (!Validate())
            {
                this.value = (false, null, "Username or password is empty");
                return;
            }

            Regex rx = new Regex(@"^[a-zA-Z0-9_-]{3,30}$", RegexOptions.Compiled);
            MatchCollection matches1 = rx.Matches(UserName);

            if (matches1.Count <= 0)
            {
                this.value = (false, null, "Invalid username. Usernames must be 3-30 characters long and can only include alphanumeric characters, underscores, and hyphens."
);
                return;
            }

            DbCinemaManagementContext context = new DbCinemaManagementContext();
            if (context.Database.CanConnect())
            {
                var person = context.Accounts.Where(acc => acc.Username == UserName).FirstOrDefault();
                if (person == null)
                    this.value =  (false, null, "Account not found");
                else if (BCrypt.Net.BCrypt.Verify(Password, person.Password))
                {
                    this.value = (true, person, "Login successfully");
                } else {
                    this.value = (false, null, "Password is incorrect");
                }
            } else this.value = (false, null, "Cannot connect to the database");

            Debug.WriteLine(this.value);
        }

    }
}
