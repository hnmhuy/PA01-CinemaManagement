﻿using CinemaManagement.Models;
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

namespace CinemaManagement.ViewModels
{
    public class AccountViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;       
        public static string MALE = "Male";
        public static string FEMALE = "Female";
        public RelayCommand SaveChanges { get; set; }
        public RelayCommand RequestAuthentication { get; set; }
        private string _gender;
        private DateTime _selectedDate;
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

        public Account userData { set; get; }

        public AccountViewModel()
        {
            //FullName = "Nguyễn Văn A";
            //Gender = "Male";
            //SelectedDate = DateTime.Now;            
            GenderList = new ObservableCollection<string>
            {
                MALE,
                FEMALE
            };
            SaveChanges = new RelayCommand(Change);
            authenticateInfo = AuthenticationControl.RestoreSession();      
            IsAuthenticated = authenticateInfo.Item1;
            if (IsAuthenticated)
            {
                var context = new DbCinemaManagementContext();
                userData = context.Accounts.Where(a => a.AccountId == authenticateInfo.Item2).FirstOrDefault();
                FullName = userData.Fullname;
            }
            IsAuthenticating = false;
            RequestAuthentication = new RelayCommand(OnRequestAuthentication, CanRequestAuthentication);
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

                this.SelectedDate = new DateTime(returnValue.Item2.Dob.Year, returnValue.Item2.Dob.Month, returnValue.Item2.Dob.Day);
                IsAuthenticating = false;
                IsAuthenticated = true;
            } else
            {
                IsAuthenticated = false;
                IsAuthenticating = false;
            }
        }

    }
}
