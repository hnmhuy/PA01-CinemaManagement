﻿using CinemaManagement.Models;
using CinemaManagement.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    public class MoviePageViewModel : INotifyPropertyChanged
    {
        private readonly DbCinemaManagementContext _context;

        private MovieViewModel _moviesList;
        public MovieViewModel MoviesList
        {
            get { return _moviesList; }
            set
            {
                _moviesList = value;
                RaisePropertyChanged(nameof(MoviesList));
            }
        }

        private GenreViewModel _genresList;
        public GenreViewModel GenresList
        {
            get { return _genresList; }
            set
            {
                _genresList = value;
                RaisePropertyChanged(nameof(GenresList));
            }
        }

        public MoviePageViewModel(MovieViewModel movieViewModel, GenreViewModel genreViewModel)
        {
            MoviesList = movieViewModel;
            GenresList = genreViewModel;
        }
        
        public MoviePageViewModel(DbCinemaManagementContext context)
        {
            _context = context;

        }

        public MoviePageViewModel()
        {
            //var context = new DbCinemaManagementContext();
            //MoviesList = new MovieViewModel(context);
            ////GenresList = new GenreViewModel();
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
        public void RefreshData()
        {
            MoviesList.RefreshDataAsync();
        }

        public bool CreateGenre(string newName)
        {
            var db = new DbCinemaManagementContext();   
            var genre = new Genre { GenreName = newName };
            db.Genres.Add(genre);
            try
            {
                db.SaveChanges();
                this.GenresList.GenresList.Add(new GenreCommand(genre, this.GenresList.DeleteCommand));
                return true;
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
}
