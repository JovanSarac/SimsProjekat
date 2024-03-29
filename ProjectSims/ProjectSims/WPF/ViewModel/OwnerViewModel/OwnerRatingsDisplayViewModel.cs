﻿using ProjectSims.Domain.Model;
using ProjectSims.Observer;
using ProjectSims.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace ProjectSims.WPF.ViewModel.OwnerViewModel
{
    public class OwnerRatingsDisplayViewModel : IObserver
    { 
        public ObservableCollection<AccommodationAndOwnerRating> Ratings { get; set; }
        private AccommodationRatingService ratingService;

        public OwnerRatingsDisplayViewModel(Owner o, NavigationService navService)
        {
            ratingService = new AccommodationRatingService();
            Ratings = new ObservableCollection<AccommodationAndOwnerRating>(ratingService.GetRatingsWhereGuestRated(o.Id));
        }
        public void Update()
        {
            Ratings.Clear();
            foreach (AccommodationAndOwnerRating rating in ratingService.GetAllRatings())
            {
                Ratings.Add(rating);
            }
        }
    }
}
