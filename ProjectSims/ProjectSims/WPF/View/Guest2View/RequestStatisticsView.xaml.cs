﻿using ProjectSims.Domain.Model;
using ProjectSims.Service;
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
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace ProjectSims.WPF.View.Guest2View
{
    /// <summary>
    /// Interaction logic for RequestStatisticsView.xaml
    /// </summary>
    public partial class RequestStatisticsView : Window
    {
        private TourRequestService tourRequestService;
        public List<TourRequest> requests { get; set; }
        public Guest2 guest2 { get; set; }
        public SeriesCollection PieSeries { get; set; }
        public Func<int, string> Values { get; set; }
        public Func<ChartPoint, string> Pointlabel { get; set; }

        public double AverageNumberOfPeople { get; set; }
        public RequestStatisticsView(Guest2 g)
        {
            InitializeComponent();
            Pointlabel = chartPoint => String.Format("{0}({1:P})", chartPoint.Y, chartPoint.Participation);
            this.DataContext = this;
            guest2 = g;
            tourRequestService = new TourRequestService();
            requests = new List<TourRequest>(tourRequestService.GetByGuest2Id(guest2.Id));

            GetRequestStatistics();
            AverageNumberOfPeople = tourRequestService.GetAverageNumberOfPeopleOnAcceptedRequests(requests);
        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void GetRequestStatistics()
        {
            PieSeries = new SeriesCollection()
            {
                new PieSeries
                {
                    Title = "prihvaceni",
                    Values = new ChartValues<ObservableValue> {
                        new ObservableValue(tourRequestService.GetNumberRequestsByState(requests, TourRequestState.Accepted)) 
                    },
                    DataLabels = true
                },

                new PieSeries
                {
                    Title = "neprihvaceni",
                    Values = new ChartValues<ObservableValue> { 
                        new ObservableValue(tourRequestService.GetNumberRequestsByState(requests, TourRequestState.Invalid) + 
                        tourRequestService.GetNumberRequestsByState(requests, TourRequestState.Waiting))
                    },
                    DataLabels = true
                }
            };
        }
    }
}