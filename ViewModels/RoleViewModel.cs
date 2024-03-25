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
using Syncfusion.UI.Xaml.Core;

namespace CinemaManagement.ViewModels
{

    public class RoleCommand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand DeleteCommand { get; set; }
        public Role Role { get; set; }

        public RoleCommand(Role _Role, RelayCommand _deleteCommand)
        {
            this.Role = _Role;
            this.DeleteCommand = _deleteCommand;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
    public class RoleViewModel: INotifyPropertyChanged
    {
        private readonly DbCinemaManagementContext _context;
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand DeleteCommand { get; set; }
        public ObservableCollection<RoleCommand> RolesList { get; set; }
        //private RoleCommand _selectedRole;
        //public RoleCommand SelectedRole
        //{
        //    get { return _selectedRole; }
        //    set
        //    {
        //        if (_selectedRole != value)
        //        {
        //            _selectedRole = value;
        //            OnPropertyChanged(nameof(SelectedRole));
        //        }
        //    }
        //}
        public RoleCommand SelectedRole { get; set; }
        public RoleViewModel(DbCinemaManagementContext context)
        {
            _context = context;
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            RolesList = GenerateRoleSampleData();
            if (RolesList.Count > 0)
                SelectedRole = RolesList[0];
        }

        private async Task DeleteRoleAsync(Role role)
        {
            try
            {
                // Remove all contributors associated with the movie
                _context.Contributors.RemoveRange(role.Contributors);
                

                // Save changes to the database
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

               
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting movie related data: {ex.Message}");
            }
        }

        private bool CanDelete(object parameter)
        {

            return SelectedRole != null;
        }

        private void OnDelete(object obj)
        {
            // Print a debug message to indicate that the OnDelete method is being called
            Debug.WriteLine("OnDelete method called.");

            // Check if SelectedMovie is correctly set
            if (SelectedRole != null)
            {
                Debug.WriteLine($"Deleting movie: {SelectedRole.Role.RoleName}");
                DeleteRoleAsync(SelectedRole.Role);
                RolesList.Remove(SelectedRole);
                // Remove the selected movie from the RolesList
            }
            else
            {
                // Print a debug message if SelectedMovie is null
                Debug.WriteLine("SelectedMovie is null. Cannot delete.");
            }

        }

        public RoleViewModel()
        {
            //RolesList = GenerateRoleSampleData();
            //SelectedRole = RolesList[1];
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<RoleCommand> GenerateRoleSampleData()
        {
            ObservableCollection<RoleCommand> roles = new ObservableCollection<RoleCommand>();

            var Roles = _context.Roles.ToList();

            foreach (var role in Roles)
            {
                roles.Add(new RoleCommand(role, DeleteCommand));
            }

            return roles;

            //Roles.Add(
            //    new Role
            //    {
            //        RoleId = 1,
            //        RoleName = "Actor"
            //    });
            //Roles.Add(
            //    new Role
            //    {
            //        RoleId = 2,
            //        RoleName = "Director"
            //    });
            //Roles.Add(
            //    new Role
            //    {
            //        RoleId = 3,
            //        RoleName = "Producer"
            //    });
            //Roles.Add(
            //    new Role
            //    {
            //        RoleId = 4,
            //        RoleName = "DOP"
            //    });
            //Roles.Add(
            //    new Role
            //    {
            //        RoleId = 5,
            //        RoleName = "Writer"
            //    });
            //Roles.Add(
            //    new Role
            //    {
            //        RoleId = 6,
            //        RoleName = "Make-up artist"
            //    });
            //return roles;
        }

        public void AddNewRole(string roleName)
        {
            Role role = new Role
            {
                RoleName = roleName
            };

            _context.Roles.Add(role);
            _context.SaveChanges();
            RolesList.Add(new RoleCommand(role, DeleteCommand));
        }

        public void UpdateRole(string roleName)
        {
            int id = SelectedRole.Role.RoleId;
            int index = RolesList.IndexOf(SelectedRole);
            Role role = _context.Roles.Find(id);
            role.RoleName = roleName;
            _context.Roles.Update(role);
            _context.SaveChanges();
            // Delete and re-add the updated role to the RolesList
            RolesList.RemoveAt(index);
            RolesList.Insert(index, new RoleCommand(role, DeleteCommand));
        }

    }
    public class TotalRoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<RoleCommand> Roles)
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
