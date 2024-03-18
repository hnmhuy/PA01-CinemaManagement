using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.Views
{
    public class NavigationHelper
    {
        private NavigationView navView;
        private Frame contentFrame;
        private Type homePage;
        private Type settingPage;

        public Type HomePage
        {
            get { return homePage; }
            set { homePage = value; }
        }

        public Frame ContentFrame
        {
            get { return contentFrame; }
            set { contentFrame = value; }
        }

        public NavigationView NavView
        {
            get { return navView; }
            set { navView = value; }
        }


        public NavigationHelper(NavigationView navView, Frame contentFrame, Type homePage, bool haveSetting = false, Type settingPage = null)
        {
            this.navView = navView;
            this.contentFrame = contentFrame;
            navView.Loaded += NavView_Loaded;
            navView.ItemInvoked += NavView_ItemInvoked;
            navView.BackRequested += NavView_BackRequested;
            navView.SelectionChanged += NavView_SelectionChanged;
            //contentFrame.Navigated += On_Navigated;
            //contentFrame.NavigationFailed += ContentFrame_NavigationFailed;
            this.homePage = homePage;
            if (haveSetting)
            {
                this.settingPage = settingPage;
            }
        }


        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate(HomePage, new EntranceNavigationTransitionInfo());
        }

        private void NavView_ItemInvoked(NavigationView sender,
                                         NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate(settingPage, args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                NavView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        // NavView_SelectionChanged is not used in this example, but is shown for completeness.
        // You will typically handle either ItemInvoked or SelectionChanged to perform navigation,
        // but not both.
        private void NavView_SelectionChanged(NavigationView sender,
                                              NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate(settingPage, args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {          
                Type navPageType = Type.GetType(args.SelectedItemContainer.Tag.ToString());
                NavView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(
            Type navPageType,
            NavigationTransitionInfo transitionInfo)
        {
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            Type preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
            {
                ContentFrame.Navigate(navPageType, null, transitionInfo);
            }
        }

        private void NavView_BackRequested(NavigationView sender,
                                           NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            //if (NavView.IsPaneOpen &&
            //    (NavView.DisplayMode == NavigationViewDisplayMode.Compact ||
            //     NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
            //    return false;

            ContentFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;
            NavView.IsBackButtonVisible = ContentFrame.CanGoBack ?
                NavigationViewBackButtonVisible.Visible :
                NavigationViewBackButtonVisible.Collapsed;

            if (ContentFrame.SourcePageType == settingPage)
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
                //NavView.Header = "Settings";
            }
            else if (ContentFrame.SourcePageType != null)
            {
                // Select the nav view item that corresponds to the page being navigated to.

                // Find the NavigationViewItem for the page by name.
                //NavigationViewItem selectedItem = NavView.MenuItems
                //    .OfType<NavigationViewItem>()
                //    .First(n => n.Tag.Equals(e.SourcePageType.FullName));

                //if (selectedItem != null)
                //{
                //    NavView.SelectedItem = selectedItem;
                //    //NavView.Header =
                //    //    ((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
                //}
                //else
                //{
                //    // If not found, collapse the NavigationView.
                //    NavView.SelectedItem = null;
                //    //NavView.Header = "Cinema Management";
                //}

                //NavView.Header =
                //    ((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();

            }
        }
    }
}
