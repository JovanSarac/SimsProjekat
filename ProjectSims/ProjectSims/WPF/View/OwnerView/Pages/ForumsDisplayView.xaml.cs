﻿using ProjectSims.Domain.Model;
using ProjectSims.WPF.ViewModel.OwnerViewModel;
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

namespace ProjectSims.WPF.View.OwnerView.Pages
{
    /// <summary>
    /// Interaction logic for ForumsDisplay.xaml
    /// </summary>
    public partial class ForumsDisplayView : Page
    {
        public ForumsDisplayView(Owner owner, OwnerStartingView window, NavigationService navService)
        {
            InitializeComponent();
            DataContext = new ForumsDisplayViewModel(owner, window, navService);
        }
    }
}
