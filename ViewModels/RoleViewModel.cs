using CinemaManagement.Models;
//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Forms;
using Windows.UI.Popups;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;

namespace CinemaManagement.ViewModels
{
    public class RoleViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Role> RolesList { get; set; }
        public Role SelectedRole { get; set; }

        public RoleViewModel()
        {
            RolesList = GenerateRoleSampleData();
            SelectedRole = RolesList[1];
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Role> GenerateRoleSampleData()
        {
            ObservableCollection<Role> Roles = new ObservableCollection<Role>();
            Roles.Add(
                new Role
                {
                    RoleId = 1,
                    RoleName = "Actor"
                });
            Roles.Add(
                new Role
                {
                    RoleId = 2,
                    RoleName = "Director"
                });
            Roles.Add(
                new Role
                {
                    RoleId = 3,
                    RoleName = "Producer"
                });
            Roles.Add(
                new Role
                {
                    RoleId = 4,
                    RoleName = "DOP"
                });
            Roles.Add(
                new Role
                {
                    RoleId = 5,
                    RoleName = "Writer"
                });
            Roles.Add(
                new Role
                {
                    RoleId = 6,
                    RoleName = "Make-up artist"
                });
            return Roles;
        }


    }
    public class TotalRoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<Role> Roles)
            {
                int total = 0;
                total = Roles.Count;
                return total;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
