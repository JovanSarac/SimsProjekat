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
using ProjectSims.WPF.View.Guest1View;
using ProjectSims.WPF.View.Guest1View.MainPages;
using ProjectSims.WPF.ViewModel.Guest1ViewModel;

namespace ProjectSims.WPF.View.Guest1View.MainPages
{
    /// <summary>
    /// Interaction logic for AccommodationReservationView.xaml
    /// </summary>
    public partial class GuestAccommodationsView : Page
    {
        public GuestAccommodationsViewModel ViewModel { get; set; }
        private Frame selectedTab;
        public Guest1 Guest { get; set; }
        public Accommodation SelectedAccommodation { get; set; }

        public GuestAccommodationsView(Guest1 guest, Frame selectedTab)
        {
            InitializeComponent();

            ViewModel = new GuestAccommodationsViewModel(guest);
            this.DataContext = ViewModel;

            Guest = guest;
            this.selectedTab = selectedTab;
        }

        public void Reservation_Click(object sender, RoutedEventArgs e)
        {
            SelectedAccommodation = (Accommodation)AccommodationsTable.SelectedItem;
            if (SelectedAccommodation != null)
            {
                ChangeTab(6);
            }
        }

        private void MyReservations_Click(object sender, RoutedEventArgs e)
        {
            ChangeTab(1);
        }
        private void RateAccommodation_Click(object sender, RoutedEventArgs e)
        {
            AccommodationsForRating accommodationForRating = new AccommodationsForRating(Guest);
            accommodationForRating.Show();    
        }

        public void ChangeTab(int tabNum)
        {
            switch (tabNum)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        selectedTab.Content = new MyReservations(Guest, selectedTab);
                        break;
                    }
                case 2:
                    {
                        break;
                    }
                case 4:
                    {
                        break;
                    }
                case 5:
                    {
                        break;
                    }
                case 6:
                    {
                        selectedTab.Content = new AccommodationReservationView(SelectedAccommodation, Guest, selectedTab);
                        break;
                    }
            }
        }

        public void Search_Click(object sender, RoutedEventArgs e)
        {
           ViewModel.Search(TextboxName.Text, TextboxCity.Text, TextboxCountry.Text, TextboxType.Text, TextboxGuests.Text, TextboxDays.Text);
        }
    }

}
