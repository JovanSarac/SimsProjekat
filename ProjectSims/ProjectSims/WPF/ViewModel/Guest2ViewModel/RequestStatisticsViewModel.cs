﻿using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using ProjectSims.Domain.Model;
using ProjectSims.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSims.WPF.ViewModel.Guest2ViewModel
{
    public class RequestStatisticsViewModel
    {
        private TourRequestService tourRequestService;
        public List<TourRequest> requests { get; set; }
        public Guest2 guest2 { get; set; }
        public SeriesCollection PieSeries { get; set; }
        public Func<int, string> Values { get; set; }
        public Func<ChartPoint, string> Pointlabel { get; set; }
        public string[] LabelsLanguage { get; set; }
        public string[] LabelsLocation { get; set; }
        public ObservableCollection<string> LabelsForLanguages { get; set; }
        public ObservableCollection<string> LabelsForLocations { get; set; }
        public SeriesCollection NumberRequestForLanguageSeriesCollection { get; set; }
        public SeriesCollection NumberRequestForLocationSeriesCollection { get; set; }
        public double AverageNumberOfPeople { get; set; }

        public RequestStatisticsViewModel(Guest2 g)
        {
            Pointlabel = chartPoint => String.Format("{0}({1:P})", chartPoint.Y, chartPoint.Participation);
            guest2 = g;
            tourRequestService = new TourRequestService();
            requests = new List<TourRequest>(tourRequestService.GetByGuest2Id(guest2.Id));

            GetRequestStatistics();
            AverageNumberOfPeople = tourRequestService.GetAverageNumberOfPeopleOnAcceptedRequests(requests);

            NumberRequestForLanguageSeriesCollection = new SeriesCollection();
            NumberRequestForLocationSeriesCollection = new SeriesCollection();
            LabelsLanguage = new[] { "Srpski", "Engleski", "Francuski", "Nemački", "Španski", "Italijanski" };

            LabelsForLanguages = new ObservableCollection<string>();
            foreach (var l in LabelsLanguage)
            {
                LabelsForLanguages.Add(l);
            }

            LabelsForLocations = new ObservableCollection<string>();
            LabelsLocation = tourRequestService.GetAllLocations(requests).ToArray();
            foreach (var l in LabelsLocation)
            {
                LabelsForLocations.Add(l);
            }

            DisplayTheNumberRequestByLanguage();
            DisplayTheNumberRequestByLocation();
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

        public void DisplayTheNumberRequestByLanguage()
        {
            var languageStats = new ChartValues<int>();
            foreach (string language in LabelsLanguage)
            {
                languageStats.Add(tourRequestService.GetNumberRequestsByLanguage(requests, language));
            }
            NumberRequestForLanguageSeriesCollection.Add(new ColumnSeries { Values = languageStats, Title = "Broj zahteva za jezik:" });
            Values = value => value.ToString("D");
        }

        public void DisplayTheNumberRequestByLocation()
        {
            var locationStats = new ChartValues<int>();
            foreach (string location in LabelsLocation)
            {
                locationStats.Add(tourRequestService.GetNumberRequestsByLocation(requests, location));
            }
            NumberRequestForLocationSeriesCollection.Add(new ColumnSeries { Values = locationStats, Title = "Broj zahteva za lokaciju:" });
            Values = value => value.ToString("D");
        }
    }
}
