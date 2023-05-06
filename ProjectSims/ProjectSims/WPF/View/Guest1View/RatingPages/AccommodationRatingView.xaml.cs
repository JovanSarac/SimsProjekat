﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectSims.Domain.Model;
using ProjectSims.WPF.ViewModel.Guest1ViewModel;

namespace ProjectSims.WPF.View.Guest1View.RatingPages
{
    /// <summary>
    /// Interaction logic for AccommodationRating.xaml
    /// </summary>
    public partial class AccommodationRatingView : Page
    {
        private AccommodationRatingViewModel viewModel;
        public AccommodationRatingView(AccommodationReservation accommodationReservation)
        {
            InitializeComponent();
            viewModel = new AccommodationRatingViewModel(accommodationReservation);
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();

            string fileName = "";
            if (result == true)
            {
                fileName = openFileDlg.SafeFileName;
            }
            else
            {
                return;
            }
            Images.Text += GetRelativePath(fileName) + ",\n";
        }

        private string GetRelativePath(string fileName)
        {
            return "../../../Resources/Images/Guest1/" + fileName;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CleanlinessTb.Text) && !string.IsNullOrEmpty(FairnessTb.Text) && !string.IsNullOrEmpty(LocationTb.Text) && !string.IsNullOrEmpty(ValueForMoneyTb.Text))
            {
                viewModel.Confirm(CleanlinessTb.Text, FairnessTb.Text, LocationTb.Text, ValueForMoneyTb.Text, CommentTb.Text, Images.Text);
            }
            NavigationService.Navigate(new RenovationRecommendationView(viewModel));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        } 
    }
}