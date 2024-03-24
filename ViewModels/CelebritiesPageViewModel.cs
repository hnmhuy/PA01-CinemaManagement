using CinemaManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{

   
    public class CelebritiesPageViewModel: INotifyPropertyChanged
    {
        private readonly DbCinemaManagementContext _context;

        private PersonViewModel _peopleList;
        public PersonViewModel PeopleList
        {
            get { return _peopleList; }
            set
            {
                _peopleList = value;
                RaisePropertyChanged("PeopleList");
            }
        }

        private RoleViewModel _rolesList;
        public RoleViewModel RolesList
        {
            get { return _rolesList; }
            set
            {
                _rolesList = value;
                RaisePropertyChanged("RolesList");
            }
        }

        public CelebritiesPageViewModel(PersonViewModel personViewModel, RoleViewModel roleViewModel)
        {
            RolesList = roleViewModel;
            PeopleList = personViewModel;
        }

        public CelebritiesPageViewModel(DbCinemaManagementContext context)
        {
            _context = context;

        }

        public CelebritiesPageViewModel()
        {
            //PeopleList = new PersonViewModel();
            //RolesList = new RoleViewModel(); 
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
