﻿using ProjectSims.Service;
using ProjectSims.Domain.Model;
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
using ProjectSims.Repository;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProjectSims.WPF.View.Guest2View.Pages;

namespace ProjectSims.WPF.View.Guest2View
{
    /// <summary>
    /// Interaction logic for RatingTourView.xaml
    /// </summary>
    public partial class RatingTourView : Window
    {
        public GuideService guideService { get; set; }
        public Guide guide { get; set; }
        public Guest2 guest2 { get; set; }

        public Tour tourRate { get; set; }
        private TourRatingService tourRatingService { get; set; }
        private ReservationTourService reservationTourService { get; set; }
        private KeyPointService keyPointService { get; set; }
        public RatingTourView(Tour tour,Guest2 g,ReservationTourService rts)
        {
            InitializeComponent();
            DataContext = this;

            guideService = new GuideService();
            guide = guideService.GetGuideById(tour.GuideId);
            guest2 = g;
            tourRate = tour;
            reservationTourService = rts;
            tourRatingService = new TourRatingService();
            keyPointService = new KeyPointService(); 
            foreach (int id in tourRate.KeyPointIds)
            {
                if (id.Equals(tourRate.KeyPointIds.Last()))
                {
                    KeyPointTextBox.Text += keyPointService.GetKeyPointById(id).Name;
                }
                else
                {
                    KeyPointTextBox.Text += keyPointService.GetKeyPointById(id).Name + ", ";
                }
            }
        }

        private void Rating_Click(object sender, RoutedEventArgs e)
        {
            int knowledgeGuide = FindRatingKnowledgeGuide();
            if (knowledgeGuide == 0)
            {
                MessageBox.Show("Morate ocijeniti znanje vodica!");
                return;
            }
            int languageGuide = FindRatinLanguageGuide();
            if (languageGuide == 0)
            {
                MessageBox.Show("Morate ocijeniti jezik vodica!");
                return;
            }
            int interestingTour = FindRatingInterestingTour();
            if (interestingTour == 0)
            {
                MessageBox.Show("Morate ocijeniti zanimljivost ture!");
                return;
            }

            List<string> imageList = new List<string>();
            if (!string.IsNullOrEmpty(ImagesBox.Text))
            {
                string images = ImagesBox.Text.Remove(ImagesBox.Text.Length - 1, 1);
                foreach (string image in images.Split(','))
                {
                    imageList.Add(image);
                }
            }
            else
            {
                imageList.Add("");
            }
            TourAndGuideRating tourRating = new TourAndGuideRating(guest2.Id, guest2, tourRate.Id, knowledgeGuide, languageGuide, interestingTour, AddedComentBox.Text, imageList);
            tourRatingService.Create(tourRating);
            ReservationTour reservation = reservationTourService.GetReservationByGuestAndTour(tourRate, guest2);
            reservation.RatedTour = true;
            reservationTourService.Update(reservation);
            Close();
        }
        private int FindRatingKnowledgeGuide()
        {
            if ((bool)RadioButton1.IsChecked)
            {
                return 1;
            }
            else if ((bool)RadioButton2.IsChecked)
            {
                return 2;
            }
            else if ((bool)RadioButton3.IsChecked)
            {
                return 3;
            }
            else if ((bool)RadioButton4.IsChecked)
            {
                return 4;
            }
            else if ((bool)RadioButton5.IsChecked)
            {
                return 5;
            }
            return 0;
        }
        private int FindRatinLanguageGuide()
        {
            if ((bool)RadioButton6.IsChecked)
            {
                return 1;
            }
            else if ((bool)RadioButton7.IsChecked)
            {
                return 2;
            }
            else if ((bool)RadioButton8.IsChecked)
            {
                return 3;
            }
            else if ((bool)RadioButton9.IsChecked)
            {
                return 4;
            }
            else if ((bool)RadioButton10.IsChecked)
            {
                return 5;
            }
            return 0;
        }
        private int FindRatingInterestingTour()
        {
            if ((bool)RadioButton11.IsChecked)
            {
                return 1;
            }
            else if ((bool)RadioButton12.IsChecked)
            {
                return 2;
            }
            else if ((bool)RadioButton13.IsChecked)
            {
                return 3;
            }
            else if ((bool)RadioButton14.IsChecked)
            {
                return 4;
            }
            else if ((bool)RadioButton15.IsChecked)
            {
                return 5;
            }
            return 0;
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();

            string apsolutePath = "";
            if (result == true)
            {
                apsolutePath = openFileDlg.FileName;
            }
            else
            {
                return;
            }
            ImagesBox.Text += GetRelativePath(apsolutePath) + ",";
        }
        private string GetRelativePath(string apsolutePath)
        {
            string[] helpString = apsolutePath.Split('\\');
            string nameFile = helpString.Last();
            return "/Resources/Images/Guest2/" + nameFile;
        }

        private void ButtonClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
