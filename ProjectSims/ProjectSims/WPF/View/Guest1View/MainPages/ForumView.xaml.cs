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
    /// Interaction logic for Forum.xaml
    /// </summary>
    public partial class ForumView : Page
    {
        public ForumView(Guest1 guest, NavigationService navigation)
        {
            this.DataContext = new ForumViewModel(guest, navigation);
            InitializeComponent();
            BackButton.Focus();
        }

        private void ForumsTable_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.S))
            {
                ShowButton.Focus();
            }
        }
    }
}
