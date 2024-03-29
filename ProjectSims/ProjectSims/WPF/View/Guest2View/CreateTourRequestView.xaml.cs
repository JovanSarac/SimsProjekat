﻿using ProjectSims.Domain.Model;
using ProjectSims.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectSims.WPF.View.Guest2View
{
    /// <summary>
    /// Interaction logic for CreateTourRequestView.xaml
    /// </summary>
    public partial class CreateTourRequestView : Window, INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _location;
        private string _description;
        private string _language;
        private int _maxNumberGuests;

        public string Location 
        { 
            get => _location;
            set 
            {
                if (value != _location)
                {
                    _location = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TourLanguage
        {
            get => _language;
            set
            {
                if (value != _language)
                {
                    _language = value;
                    OnPropertyChanged();
                }
            }
        }
        public int MaxNumberGuests
        {
            get => _maxNumberGuests;
            set
            {
                if (value != _maxNumberGuests)
                {
                    _maxNumberGuests = value;
                    OnPropertyChanged();
                }
            }
        }

        //private Regex locationRegex = new Regex("^[A-Za-z]+,[A-Za-z]+$");
        private Regex locationRegex = new Regex(@"^[A-Za-z\s]+,[A-Za-z\s]+$");
        public string Error => null;
        public string this[string columnName]
        {
            get
            {
                if (columnName == "Location")
                {
                    if (string.IsNullOrEmpty(Location))
                        return "Unesite lokaciju ture!";
                    Match match = locationRegex.Match(Location);
                    if (!match.Success)
                        return "Format mora biti Grad,Drzava (npr. Beograd,Srbija)!";
                }
                else if (columnName == "Description")
                {
                    if (string.IsNullOrEmpty(Description))
                        return "Unesite opis ture!";
                }
                else if (columnName == "TourLanguage")
                {
                    if (string.IsNullOrEmpty(TourLanguage))
                        return "Odaberite jezik!";
                }
                else if(columnName == "MaxNumberGuests" )
                {
                    if (MaxNumberGuests == 0)
                        return "Unesite broj gostiju!";
                }
                return null;

            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        public Guest2 guest2 { get; set; }
        private TourRequestService tourRequestService { get; set; }
        public CreateTourRequestView(Guest2 g)
        {
            InitializeComponent();
            this.DataContext = this;
            guest2 = g;
            tourRequestService = new TourRequestService();
            DateStart.BlackoutDates.Add(new CalendarDateRange(DateTime.Today,DateTime.Today.AddDays(2)));
        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DateStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateEnd.IsEnabled = true;
            DateEnd.Text = "";
            DateEnd.DisplayDateStart = DateTime.Parse(DateStart.Text);
        }

        private readonly string[] _validatedProperties = { "Location", "Description", "TourLanguage", "MaxNumberGuests"};
        public bool IsValid
        {
            get
            {
                foreach (var property in _validatedProperties)
                {
                    if (this[property] != null)
                        return false;
                }
                return true;
            }
        }

        private void ButtonCreateTourRequest(object sender, RoutedEventArgs e)
        {
            if (IsValid)
            {   
                if(DateStart.Text == "" || DateEnd.Text == "")
                {
                    MessageBox.Show("Morate popuniti opseg datuma!");
                    return;
                }
                TourRequest tourRequest = new TourRequest(guest2.Id,-1,TourRequestState.Waiting,Location,Description,TourLanguage,MaxNumberGuests,DateOnly.Parse(DateStart.Text), DateOnly.Parse(DateEnd.Text),false);
                tourRequestService.Create(tourRequest);
                Close();
            }
            else
                MessageBox.Show("Nisu popunjena sva polja!");
            
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                MaxNumberGuests++;
                e.Handled = true;
            }
            else if (e.Key == Key.Down && MaxNumberGuests > 0)
            {
                MaxNumberGuests--;
                e.Handled = true;
            }
            else if (e.Key == Key.Back && MaxNumberGuests < 10)
            {
                MaxNumberGuests = 0;
                e.Handled = true;
            }
            else if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                MaxNumberGuests = 0;
                e.Handled = true;
            }
        }
    }
}
