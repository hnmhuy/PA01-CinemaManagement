using CinemaManagement.Models;
using CinemaManagement.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CinemaManagement.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowsePage : Page
    {

        private DispatcherTimer dispatcherTimer;
        private FrameworkElement displayTarget;
        private Flyout flyoutCard;
        private MediaPlayer mediaPlayer;
        public BrowseViewModel viewModel { get; set; } = new BrowseViewModel();

        public BrowsePage()
        {
            this.InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1.2);
            DataContext = viewModel;
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            // Set the highlighting movie as the data which attached to the displayTarget
            dispatcherTimer.Stop();
            (flyoutCard.Content as Grid).DataContext = displayTarget.DataContext;
            viewModel.HighlightingMovie = displayTarget.DataContext as Movie;

            FlyoutShowOptions positions = new FlyoutShowOptions();
            double height = displayTarget.ActualHeight;
            positions.Position = new Point(112.5, height + 50);
            positions.ShowMode = FlyoutShowMode.Transient;
            flyoutCard.ShowAt(displayTarget, positions);


        }

        private void MovieCard_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            dispatcherTimer.Start();
            displayTarget = sender as FrameworkElement;
            flyoutCard = FlyoutBase.GetAttachedFlyout(displayTarget) as Flyout;
        }

        private void MovieCard_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void HighlightedMovieCard_Opening(object sender, object e)
        {

        }

        private void HighlightedMovieCard_Opened(object sender, object e)
        {
            Grid highlightContent = flyoutCard.Content as Grid;

            MediaPlayerElement temp = highlightContent.FindName("TrailerVideo") as MediaPlayerElement;
            
            // Update video source

            temp.Source = MediaSource.CreateFromUri(new Uri("ms-appx://" + viewModel.HighlightingMovie.TrailerPath));

            if (temp != null)
            {
                mediaPlayer = temp.MediaPlayer;
                mediaPlayer.Play();
                mediaPlayer.Position = TimeSpan.FromSeconds(0);
                mediaPlayer.IsLoopingEnabled = true;
            }
        }

        private void HighlightedMovieCard_Closed(object sender, object e)
        {
            mediaPlayer.Pause();
        }

        private void HighlightCardContent_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (flyoutCard != null)
            {
                flyoutCard.Hide();
            }
        }

        private void HighlightedMovieCard_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            mediaPlayer.Pause();
        }

        private void Muted_Checked(object sender, RoutedEventArgs e)
        {
            mediaPlayer.IsMuted = true;
        }

        private void Muted_Unchecked(object sender, RoutedEventArgs e)
        {
            mediaPlayer.IsMuted = false;
        }
    }
}
