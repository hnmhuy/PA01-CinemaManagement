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

    public class GenreCommand : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand DeleteCommand { get; set; }
        private Genre _genre { get; set; }

        public Genre Genre
        {
            get => _genre;
            set
            {
                if (_genre != value)
                {
                    _genre = value;
                    OnPropertyChanged(nameof(Genre));
                }
            }
        }

        public GenreCommand(Genre _Genre, RelayCommand _deleteCommand)
        {
            this.Genre = _Genre;
            this.DeleteCommand = _deleteCommand;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }

    public class GenreViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<GenreCommand> GenresList { get; set; }
        
        private GenreCommand _selectedGenre;
        public GenreCommand SelectedGenre
        {
            get { return _selectedGenre; }
            set
            {
                if (_selectedGenre != value)
                {
                    _selectedGenre = value;
                    OnPropertyChanged(nameof(SelectedGenre));
                }
            }
        }

        private readonly DbCinemaManagementContext _context;
        public RelayCommand DeleteCommand { get; }


        public GenreViewModel(DbCinemaManagementContext context)
        {
            _context = context;
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            GenresList = GenerateGenreSampleData(DeleteCommand);
            if (GenresList.Count > 0)
                SelectedGenre = GenresList[0];
        }

        public GenreViewModel()
        {
            
            //GenresList = GenerateGenreSampleData();
            //SelectedGenre = GenresList[1];
        }

        private async Task DeleteGenreFromDatabaseAsync(Genre genre)
        {
            try
            {
                // Retrieve all movies associated with the genre
                var movies = _context.Movies.Where(m => m.Genres.Any(g => g.GenreId == genre.GenreId)).ToList();

                // Remove the genre from each movie's genre list
                foreach (var movie in movies)
                {
                    movie.Genres.Remove(genre);
                }

                // Remove the genre from the context
                _context.Genres.Remove(genre);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting genre: {ex.Message}");
            }
        }

        private bool CanDelete(object parameter)
        {

            return SelectedGenre != null;
        }

        private void OnDelete(object obj)
        {
            // Print a debug message to indicate that the OnDelete method is being called
            Debug.WriteLine("OnDelete method called.");

            // Check if SelectedGenre is correctly set
            if (SelectedGenre != null)
            {
                Debug.WriteLine($"Deleting Genre: {SelectedGenre.Genre.GenreName}");

                DeleteGenreFromDatabaseAsync(SelectedGenre.Genre); // Call the method to delete from the database

                GenresList.Remove(SelectedGenre);
            }
            else
            {
                // Print a debug message if SelectedGenre is null
                Debug.WriteLine("SelectedGenre is null. Cannot delete.");
            }

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection <GenreCommand> GenerateGenreSampleData(RelayCommand DeleteCommand)
        {
            ObservableCollection<GenreCommand> genres = new ObservableCollection<GenreCommand>();
            var Genres = _context.Genres.ToList();

            foreach (var genre in Genres)
            {
                genres.Add(new GenreCommand(genre,DeleteCommand));
            }

            return genres;
        }

        public void UpdateGenre(string newName)
        {
            var index = GenresList.IndexOf(SelectedGenre);
            var id = SelectedGenre.Genre.GenreId;
            SelectedGenre.Genre.GenreName = newName;
            _context.SaveChanges();
            // Remove and re-add the genre to the list to trigger the UI update
            GenresList.RemoveAt(index);
            var newGenre = _context.Genres.Find(id);    
            GenresList.Insert(index, new GenreCommand(newGenre, DeleteCommand));
        }


    }
    public class TotalGenreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ObservableCollection<Genre> genres)
            {
                int total = 0;
                total = genres.Count;
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
