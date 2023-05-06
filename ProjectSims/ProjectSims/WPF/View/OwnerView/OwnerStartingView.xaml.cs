﻿using ProjectSims.Domain.Model;
using ProjectSims.View.OwnerView;
using ProjectSims.WPF.View.Guest2View.Pages;
using ProjectSims.WPF.View.OwnerView.Pages;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ProjectSims.Service;
using ProjectSims.View.OwnerView.Pages;
using System.IO;

namespace ProjectSims.WPF.View.OwnerView
{
    /// <summary>
    /// Interaction logic for OwnerStartingView.xaml
    /// </summary>
    public partial class OwnerStartingView : Window
    {
        public Owner owner { get; set; }
        private GuestRatingService guestRatingService { get; set; }
        
        public OwnerStartingView(Owner o)
        {
            InitializeComponent();
            DataContext = this;
            owner = o; 
            SelectedTab.Content = new HomePage(owner);
            guestRatingService = new GuestRatingService();
       }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            guestRatingService.NotifyOwnerAboutRating();
        }

        private void ButtonMenu(object sender, RoutedEventArgs e)
        {
            ChangeTab(0);
        }

        private void ButtonRequests(object sender, RoutedEventArgs e)
        {
            ChangeTab(1);
        }

        private void ButtonNotifications(object sender, RoutedEventArgs e)
        {
            ChangeTab(2);
        }

        public void ChangeTab(int tabNum)
        {
            switch (tabNum)
            {
                case 0:
                    {
                        SelectedTab.Content = new SideMenu(owner);
                        break;
                    }
                case 1:
                    {
                        SelectedTab.Content = new Requests(owner);
                        break;
                    }
                case 2:
                    {
                        //SelectedTab.Content = new Notifications(owner);
                        break;
                    }
            }
        }
    }
}