﻿using ProjectSims.Domain.Model;
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
    /// Interaction logic for ReportToGenerate.xaml
    /// </summary>
    public partial class ReportToGenerateView : Page
    {
        public ReportToGenerateView(Owner owner)
        {
            InitializeComponent();
            OwnerName.Text = owner.Name + " " + owner.Surname;
            
        }
    }
}
