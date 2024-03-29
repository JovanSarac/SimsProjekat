﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private List<string> images;
        public Image image { get; set; }

        public AccommodationRatingView(AccommodationReservation accommodationReservation)
        {
            InitializeComponent();
            viewModel = new AccommodationRatingViewModel(accommodationReservation);
            images = new List<string>();

            this.DataContext = viewModel;

            CleanlinessCb.Focus();
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
            images.Add(GetRelativePath(fileName));
            LoadImages(images.Last());
        }

        private string GetRelativePath(string fileName)
        {
            return "/Resources/Images/Guest1/" + fileName;
        }
        private void LoadImages(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            image = new Image();
            image.Source = bitmapImage;
            image.Height = 200;
            image.Width = 270;
            ImageList.Items.Add(image);
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.IsValid)
            {
                viewModel.AddRating(CommentTb.Text, images);
                NavigationService.Navigate(new RenovationRecommendationView(viewModel));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }    
    }
}
