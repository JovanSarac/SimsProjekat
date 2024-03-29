﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectSims.Domain.Model;
using ProjectSims.Observer;
using ProjectSims.Service;
using ProjectSims.WPF.View.Guest1View.NotifAndHelp;

namespace ProjectSims.WPF.View.Guest1View.MainPages
{
    /// <summary>
    /// Interaction logic for RatingsView.xaml
    /// </summary>
    public partial class RatingsView : Page, IObserver
    {
        public ObservableCollection<AccommodationAndOwnerRating> MyRatings { get; set; }
        public ObservableCollection<GuestRating> OwnerRatings { get; set; }
        private AccommodationReservationService reservationService;
        private AccommodationRatingService accommodationRatingService;
        private GuestRatingService guestRatingService;
        public Guest1 Guest { get; set; }
        public RatingsView(Guest1 guest)
        {
            InitializeComponent();
            DataContext = this;
            Guest = guest;

            reservationService = new AccommodationReservationService();
            accommodationRatingService = new AccommodationRatingService();
            guestRatingService = new GuestRatingService();

            reservationService.Subscribe(this);
            accommodationRatingService.Subscribe(this);
            guestRatingService.Subscribe(this);

            OwnerRatings = new ObservableCollection<GuestRating>(guestRatingService.GetAllRatingsForGuest(guest.Id));
            MyRatings = new ObservableCollection<AccommodationAndOwnerRating>(accommodationRatingService.GetAllRatingsByGuestId(guest.Id));
            

            BackButton.Focus();
        }

        private void Theme_Click(object sender, RoutedEventArgs e)
        {
            App app = (App)Application.Current;

            if (App.IsDark)
            {
                app.ChangeTheme(new Uri("Themes/Light.xaml", UriKind.Relative));
                App.IsDark = false;
            }
            else
            {
                app.ChangeTheme(new Uri("Themes/Dark.xaml", UriKind.Relative));
                App.IsDark = true;
            }
        }

        private void ButtonLanguage_Click(object sender, RoutedEventArgs e)
        {
            App app = (App)Application.Current;

            if (App.CurrentLanguage == "sr-LATN")
            {
                app.ChangeLanguage("en-US");
                App.CurrentLanguage = "en-US";
            }
            else
            {
                app.ChangeLanguage("sr-LATN");
                App.CurrentLanguage = "sr-LATN";
            }
        }

        private void ButtonNotifications_Click(object sender, RoutedEventArgs e)
        {
            NotificationsView notificationsView = new NotificationsView(Guest);
            notificationsView.Show();
        }

        private void MyRatings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AccommodationAndOwnerRating accommodationAndOwnerRating = (AccommodationAndOwnerRating)MyRatingsTable.SelectedItem;
            MyRatingsTb.Text = accommodationAndOwnerRating.Comment;
            MyRatingsNameLabel.Content = accommodationAndOwnerRating.Reservation.Accommodation.Name;
        }

        private void OwnerRatings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GuestRating guestRating = (GuestRating)OwnerRatingsTable.SelectedItem;
            OwnerRatingsTb.Text = guestRating.Comment;
            OwnerRatingsNameLabel.Content = guestRating.Reservation.Accommodation.Name;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        public void LogOut_Click(object sender, RoutedEventArgs e)
        {
            var login = new MainWindow();
            login.Show();
            Window parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }

        public void Update()
        {
            MyRatings.Clear();
            foreach (var myRating in accommodationRatingService.GetAllRatingsByGuestId(Guest.Id))
            {
                MyRatings.Add(myRating);
            }

            OwnerRatings.Clear();
            foreach (var ownerRating in guestRatingService.GetAllRatingsForGuest(Guest.Id))
            {
                OwnerRatings.Add(ownerRating);
            }
        }
    }
}
