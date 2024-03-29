﻿using ProjectSims.Domain.Model;
using ProjectSims.Service;
using ProjectSims.WPF.ViewModel.GuideViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectSims.WPF.View.GuideView.Pages
{
    /// <summary>
    /// Interaction logic for SuggestionsView.xaml
    /// </summary>
    public partial class SuggestionsView : Page
    {
        private SuggestionsViewModel suggestionsViewModel;
        public Guide Guide { get; set; }
        public SuggestionsView(Guide g)
        {
            InitializeComponent();
            suggestionsViewModel = new SuggestionsViewModel(g,LanguageHyperLink,LocationHyperLink);
            this.DataContext = suggestionsViewModel;
            Guide = g;
        }
        private void CreateTourByLanguage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CreateTourView(Guide,null,suggestionsViewModel.GetMostWantedLanguage(),null,new DateTime(0001,01,01),0));
        }
        private void CreateTourByLocation_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CreateTourView(Guide, null, null, suggestionsViewModel.GetMostWantedLocation(), new DateTime(0001, 01, 01), 0));
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(this.Parent);
        }
    }
}
