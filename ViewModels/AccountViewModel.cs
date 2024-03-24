using CinemaManagement.Models;
using CinemaManagement.WindowViews;
using LiveChartsCore.Defaults;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CinemaManagement.ViewModels
{
    public class AccountViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;       
        public static string MALE = "Male";
        public static string FEMALE = "Female";

        public RelayCommand SignOut { get; set; }
        public RelayCommand SaveChanges { get; set; }
        public RelayCommand RequestAuthentication { get; set; }
        public RelayCommand ResetPassword { get; set; }
        private string _gender;
        private DateTimeOffset? _selectedDate;
        public ObservableCollection<string> GenderList { get; set; }
        public (bool, int, string) authenticateInfo;
        public (bool, Account, string) returnValue;
        public AuthenticateWindow authenticateWindow;
        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set
            {
                _isAuthenticated = value;
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }
        private bool _isAuthenticating;
        public bool IsAuthenticating
        {
            get { return _isAuthenticating; }
            set
            {
                _isAuthenticating = value;
                OnPropertyChanged(nameof(IsAuthenticating));
            }
        }

        private bool _isLoggingOut;
        public bool IsLoggingOut
        {
            get { return _isLoggingOut; }
            set
            {
                _isLoggingOut = value;
                OnPropertyChanged(nameof(IsLoggingOut));
            }
        }
        public DateTimeOffset? SelectedDate
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
        private string _currentPassword;
        public string CurrentPassword
        {
            get { return _currentPassword; }
            set
            {
                _currentPassword = value;
                OnPropertyChanged(nameof(CurrentPassword));
            }
        }
        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }
        private string _confirmNewPassword;
        public string ConfirmNewPassword
        {
            get { return _confirmNewPassword; }
            set
            {
                _confirmNewPassword = value;
                OnPropertyChanged(nameof(ConfirmNewPassword));
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Account userData { set; get; }

        public AccountViewModel()
        {
            //FullName = "Nguyễn Văn A";
            //Gender = "Male";
            GenderList = new ObservableCollection<string>
            {
                MALE,
                FEMALE
            };
            authenticateInfo = AuthenticationControl.RestoreSession();
            IsAuthenticated = authenticateInfo.Item1;
            if (IsAuthenticated)
            {
                var context = new DbCinemaManagementContext();
                userData = context.Accounts.Where(a => a.AccountId == authenticateInfo.Item2).FirstOrDefault();
                FullName = userData.Fullname;
                Gender = userData.Gender;
            }
            IsAuthenticating = false;
            RequestAuthentication = new RelayCommand(OnRequestAuthentication, CanRequestAuthentication);
            SaveChanges = new RelayCommand(Change);
            ResetPassword = new RelayCommand(ChangePassword);
            SignOut = new RelayCommand(LogOutUser);
        }
        public void LogOutUser(object obj)
        {

            if (IsAuthenticated)
            {
                Debug.WriteLine("Function runs");
                AuthenticationControl.DestroySession();
                IsLoggingOut = true;
            }
        }
        public  void ChangePassword(object obj)
        {
            Debug.WriteLine(CurrentPassword);
            Debug.WriteLine(NewPassword);
            Debug.WriteLine(ConfirmNewPassword);
            DbCinemaManagementContext context = new DbCinemaManagementContext();
            if (context.Database.CanConnect())
            {
                var account = context.Accounts.Where(a => a.AccountId == authenticateInfo.Item2).FirstOrDefault();
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(CurrentPassword, account.Password))
                    {
                        if (NewPassword == ConfirmNewPassword)
                        {
                            account.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword);
                            Debug.WriteLine("Success");
                        }
                        else
                        {
                            Debug.WriteLine("failed here");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("failed");
                    }
                } catch
                {
                    Debug.WriteLine("failed");
                }
                
                context.SaveChanges();
            }
        }

        public void Change(object obj)
        {
            Debug.WriteLine("Change function run");
            DbCinemaManagementContext context = new DbCinemaManagementContext();
            if (context.Database.CanConnect())
            {
                var account = context.Accounts.Where(a => a.AccountId == authenticateInfo.Item2).FirstOrDefault();
                if (account != null)
                {
                    account.Fullname = FullName;
                    account.Gender = Gender;
                }
                context.SaveChanges();
            }
        }

        private bool CanRequestAuthentication(object obj)
        {
            return true;
        }

        private void OnRequestAuthentication(object obj)
        {
            IsAuthenticating = true;
            authenticateWindow = new AuthenticateWindow();
            authenticateWindow.Activate();
            authenticateWindow.Closed += OnAuthenticateWindowClosed;                                     
        }

        private void OnAuthenticateWindowClosed(object sender, WindowEventArgs args)
        {
            returnValue = authenticateWindow.returnValue;
            Debug.WriteLine("Returned value: " + returnValue.Item1);
            Debug.WriteLine("Returned value: " + (returnValue.Item2 as Account).Username);
            authenticateInfo = (returnValue.Item1, returnValue.Item2.AccountId, returnValue.Item3);
            if (returnValue.Item1)
            {
                this.FullName = returnValue.Item2.Fullname;
                this.Gender = returnValue.Item2.Gender;
                // Create a date time from string
                this.SelectedDate = new DateTimeOffset(returnValue.Item2.Dob);
                //this.SelectedDate = new DateTime(returnValue.Item2.Dob.Year, returnValue.Item2.Dob.Month, returnValue.Item2.Dob.Day);
                IsAuthenticating = false;
                IsAuthenticated = true;
                IsLoggingOut = false;
            } else
            {
                IsAuthenticated = false;
                IsAuthenticating = false;
                IsLoggingOut = false;
            }
        }

    }
}
