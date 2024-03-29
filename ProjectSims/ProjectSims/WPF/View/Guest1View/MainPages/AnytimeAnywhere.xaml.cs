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

namespace ProjectSims.WPF.View.Guest1View.MainPages
{
    /// <summary>
    /// Interaction logic for AnytimeAnywhere.xaml
    /// </summary>
    public partial class AnytimeAnywhere : Page
    {
        AnywhereAnytimeViewModel viewModel;
        public NavigationService NavService { get; set; }

        public AnytimeAnywhere(Guest1 guest, NavigationService navigation)
        {
            InitializeComponent();
            viewModel = new AnywhereAnytimeViewModel(guest, navigation, this);
            this.DataContext = viewModel;
            NavService = navigation;
            BackButton.Focus();
        }

        private void ShowDates(object sender, KeyEventArgs e)
        {
            if ((e.Key.Equals(Key.Enter)) || (e.Key.Equals(Key.Return)))
            {
                viewModel.ShowDates();
            }
        }

        private void Reserve(object sender, KeyEventArgs e)
        {
            if ((e.Key.Equals(Key.Enter)) || (e.Key.Equals(Key.Return)))
            {
                ConfirmBtn.Focus();
            }
        }
    }
}
