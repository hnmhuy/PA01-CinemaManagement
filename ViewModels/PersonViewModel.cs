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
using Microsoft.EntityFrameworkCore;
using Syncfusion.UI.Xaml.Core;


namespace CinemaManagement.ViewModels
{

    public class PersonCommand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand DeleteCommand { get; set; }
        public Person Person { get; set; }

        public PersonCommand(Person _Person, RelayCommand _deleteCommand)
        {
            this.Person = _Person;
            this.DeleteCommand = _deleteCommand;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
    public class PersonViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<PersonCommand> PeopleList { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        private readonly DbCinemaManagementContext _context;
        private PersonCommand _selectedPerson;
        public PersonCommand SelectedPerson
        {
            get { return _selectedPerson; }
            set
            {
                if (_selectedPerson != value)
                {
                    _selectedPerson = value;
                    OnPropertyChanged(nameof(SelectedPerson));
                }
            }
        }

        public PersonViewModel(DbCinemaManagementContext context)
        {
            _context = context;
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            PeopleList = GeneratePersonSampleData(DeleteCommand);
            if (PeopleList.Count > 0)
            SelectedPerson = PeopleList[0];
        }


        public PersonViewModel()
        {
            //DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            //PeopleList = GeneratePersonSampleData();
            //SelectedPerson = PeopleList[1];
        }

        private bool CanDelete(object parameter)
        {

            return SelectedPerson != null;
        }
        private async Task DeletePersonAsync(Person person)
        {
            try
            {
                // Remove all contributors associated with the movie
                _context.Contributors.RemoveRange(person.Contributors);


                // Clear the genres associated with the movie

                // Save changes to the database
                _context.People.Remove(person);
                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting movie related data: {ex.Message}");
            }
        }

        private void OnDelete(object obj)
        {
            // Print a debug message to indicate that the OnDelete method is being called
            Debug.WriteLine("OnDelete method called.");

            // Check if SelectedMovie is correctly set
            if (SelectedPerson != null)
            {
                Debug.WriteLine($"Deleting movie: {SelectedPerson.Person.Fullname}");
                DeletePersonAsync(SelectedPerson.Person);
                PeopleList.Remove(SelectedPerson);
                // Remove the selected movie from the MoviesList
            }
            else
            {
                // Print a debug message if SelectedMovie is null
                Debug.WriteLine("SelectedMovie is null. Cannot delete.");
            }

        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<PersonCommand> GeneratePersonSampleData(RelayCommand DeleteCommand)
        {
            ObservableCollection<PersonCommand> peoples = new ObservableCollection<PersonCommand>();
            var Peoples = _context.People.ToList();

            foreach (var people in Peoples)
            {
                peoples.Add(new PersonCommand(people, DeleteCommand));
            }


            //people.Add(
            //    new Person
            //    {
            //        PersonId = 1,
            //        Fullname = "Kim Taeyeon",
            //        AvatarPath = "/Assets/Images/Person/KimTaeyeon.jpg",
            //        Biography = "Kim Tae-yeon (Korean: 김태연; born March 9, 1989), known mononymously as Taeyeon, is a South Korean singer. She debuted as a member of girl group Girls' Generation in August 2007, which went on to become one of the best-selling artists in South Korea and one of the most popular K-pop groups worldwide. She has since participated in other SM Entertainment projects, including Girls' Generation-TTS, SM the Ballad, Girls' Generation-Oh!GG, and the supergroup Got the Beat."
            //    });
            //people.Add(
            //    new Person
            //    {
            //        PersonId = 2,
            //        Fullname = "Kim Ji Won",
            //        AvatarPath = "/Assets/Images/Person/KimJiWon.jpg",
            //        Biography = "Kim Ji-won is a South Korean actress. She gained attention through her roles in television series The Heirs and Descendants of the Sun"
            //    });
            //people.Add(
            //   new Person
            //   {
            //       PersonId = 3,
            //       Fullname = "Kim Soo Hyun",
            //       AvatarPath = "/Assets/Images/Person/KimSooHyun.jpg",
            //       Biography = "Kim Soo-hyun is a South Korean actor. One of the highest-paid actors in South Korea, his accolades include four Baeksang Arts Awards, two Grand Bell Awards and one Blue Dragon Film Award. "
            //   });
            //people.Add(
            //    new Person
            //    {
            //        PersonId = 4,
            //        Fullname = "Ji Chang Wook",
            //        AvatarPath = "/Assets/Images/Person/JiChangWook.jpg",
            //        Biography = "Ji Chang-wook is a South Korean actor and singer. He rose to fame for playing the lead role of Dong-hae in daily drama series Smile Again."
            //    });
            //people.Add(
            //    new Person
            //    {
            //        PersonId = 5,
            //        Fullname = "Park Seo Jun",
            //        AvatarPath = "/Assets/Images/Person/ParkSeoJun.jpg",
            //        Biography = "Park Yong-kyu, known professionally as Park Seo-joon, is a South Korean actor. He is best known for his starring roles in the television series Kill Me, Heal Me, She Was Pretty, Hwarang: The Poet Warrior Youth, Fight for My Way, What's Wrong with Secretary Kim, Itaewon Class and Gyeongseong Creature."
            //    });
            //people.Add(
            //   new Person
            //   {
            //       PersonId = 6,
            //       Fullname = "Park Bo Gum",
            //       AvatarPath = "/Assets/Images/Person/ParkBoGum.jpg",
            //       Biography = "He then starred as one of the leads in the third installment of the Reply series where he played the genius Go-player Choi Taek in Reply 1988 (2015)."
            //   });
            //people.Add(
            //   new Person
            //   {
            //       PersonId = 7,
            //       Fullname = "Cha Eun Woo",
            //       AvatarPath = "/Assets/Images/Person/ChaEunWoo.jpg",
            //       Biography = "Lee Dong-min, known professionally as Cha Eun-woo, is a South Korean singer and actor under the label Fantagio. He is a member of the South Korean boy band Astro."
            //   });
            return peoples;
        }


    }
    public class TotalPeopleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<PersonCommand> people)
            {
                int total = 0;
                total = people.Count;
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
