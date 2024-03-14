using CinemaManagement.Models;
using CinemaManagement.ViewModels;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Display.Core;
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
        private Compositor _compositor;
        SpringVector3NaturalMotionAnimation _springAnimation;

        private Frame frame;

        private DispatcherTimer dispatcherTimer;
        private FrameworkElement displayTarget;
        private Flyout flyoutCard;
        private MediaPlayer mediaPlayer;
        public BrowseViewModel viewModel { get; set; } = new BrowseViewModel();

        public BrowsePage()
        {
            this.InitializeComponent();
            // Get the Compositor from the XAML host
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            DataContext = viewModel;
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            // Set the highlighting movie as the data which attached to the displayTarget
            dispatcherTimer.Stop();
            

            // There are two kinds of DataContext in the flyout, one is RankMovie, the other is Movie
            if (displayTarget.DataContext is RankMovie)
            {
                viewModel.HighlightingMovie = (displayTarget.DataContext as RankMovie).Movie;
            }
            else
            {
                viewModel.HighlightingMovie = displayTarget.DataContext as Movie;
            }
            (flyoutCard.Content as Grid).DataContext = viewModel.HighlightingMovie;

            FlyoutShowOptions positions = new FlyoutShowOptions();
            positions.Position = CalculateDisplayPosition();
            positions.ShowMode = FlyoutShowMode.Auto;
            flyoutCard.ShowAt(frame, positions);

        }

        private Point CalculateDisplayPosition()
        {
            frame = (this.Parent as Frame);
            double frameWidth = frame.ActualWidth;
            double frameHeight = frame.ActualHeight;
            Debug.WriteLine(frameWidth + " " + frameHeight);


            double flyoutWidth = 450;
            double flyoutHeight = 500;

            Debug.WriteLine(flyoutWidth + " " + flyoutHeight);

            Point displayTargetPoint = displayTarget.TransformToVisual(frame).TransformPoint(new Point(0, 0));

            Debug.WriteLine(displayTargetPoint.X + " " + displayTargetPoint.Y);

            Point predictPoint = new Point(displayTargetPoint.X + displayTarget.ActualWidth / 2, displayTargetPoint.Y + displayTarget.ActualHeight + 40);
            if (predictPoint.X > frameWidth - flyoutWidth / 2 ) predictPoint.X = frameWidth - flyoutWidth /2;
            //if (predictPoint.Y > frameHeight - flyoutHeight / 2) predictPoint.Y = frameHeight - flyoutHeight / 2;
            Debug.WriteLine(predictPoint.X + " " + predictPoint.Y);
            return predictPoint;
        }



        private void MovieCard_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            dispatcherTimer.Start();
            displayTarget = sender as FrameworkElement;

            flyoutCard = FlyoutBase.GetAttachedFlyout(displayTarget) as Flyout;
            CreateOrUpdateSpringAnimation(0.98f, 150);
            FrameworkElement card = sender as FrameworkElement;

            (sender as UIElement).CenterPoint = new Vector3((float)(card.ActualWidth / 2.0), (float)(card.ActualHeight / 2.0), 1f);
            (sender as FrameworkElement).StartAnimation(_springAnimation);
        }

        private void MovieCard_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            CreateOrUpdateSpringAnimation(1f, 150);
            FrameworkElement card = sender as FrameworkElement;

            (sender as UIElement).CenterPoint = new Vector3((float)(card.ActualWidth / 2.0), (float)(card.ActualHeight / 2.0), 1f);
            (sender as FrameworkElement).StartAnimation(_springAnimation);
        }

        private void HighlightedMovieCard_Opening(object sender, object e)
        {

        }

        private void HighlightedMovieCard_Opened(object sender, object e)
        {
            Grid highlightContent = flyoutCard.Content as Grid;

            MediaPlayerElement temp = highlightContent.FindName("TrailerVideo") as MediaPlayerElement;
            
            // Update video source


            if (temp != null)
            {
                temp.Source = MediaSource.CreateFromUri(new Uri("ms-appx://" + viewModel.HighlightingMovie.TrailerPath));
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

        private void CreateOrUpdateSpringAnimation(float finalValue, int duration)
        {
            if (_springAnimation == null)
            {
                _springAnimation = _compositor.CreateSpringVector3Animation();
                _springAnimation.Target = "Scale";
                _springAnimation.DampingRatio = 0.8f;
            }

            _springAnimation.Period = TimeSpan.FromMilliseconds(duration);
            _springAnimation.FinalValue = new Vector3(finalValue);
        }


    }
}
