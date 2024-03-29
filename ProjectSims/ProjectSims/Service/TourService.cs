﻿using ProjectSims.FileHandler;
using ProjectSims.Domain.Model;
using ProjectSims.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using ProjectSims.Observer;
using System.Globalization;
using ProjectSims.View;
using ProjectSims.Domain.RepositoryInterface;
using System.ComponentModel.Design;
using System.Windows.Automation.Peers;

namespace ProjectSims.Service
{
    public class TourService
    {
        private ITourRepository tourRepository;
        private IGuideRepository guideRepository;
        private IKeyPointRepository keyPointRepository;
        private ITourRequestRepository tourRequestRepository;
        private IRequestForComplexTourRepository requestForComplexTourRepository;
        private ReservationTourService reservationService;

        public TourService()
        {
            tourRepository = Injector.CreateInstance<ITourRepository>();
            guideRepository = Injector.CreateInstance<IGuideRepository>();
            keyPointRepository = Injector.CreateInstance<IKeyPointRepository>();
            tourRequestRepository = Injector.CreateInstance<ITourRequestRepository>();
            requestForComplexTourRepository = Injector.CreateInstance<IRequestForComplexTourRepository>();
            reservationService = new ReservationTourService();
            InitializeGuide();
            InitializeKeyPoints();
        }
        public void InitializeGuide()
        {
            foreach (var item in tourRepository.GetAll())
            {
                item.Guide = guideRepository.GetById(item.GuideId);
            }
        }
        public void InitializeKeyPoints()
        {
            foreach (var item in tourRepository.GetAll())
            {
                if (item.KeyPoints.Count() != 0)
                {
                    item.KeyPoints.Clear();
                }
                foreach (int id in item.KeyPointIds)
                {
                    item.KeyPoints.Add(keyPointRepository.GetById(id));
                }
                item.ActiveKeyPoint = keyPointRepository.GetById(item.ActiveKeyPointId);
            }
        }
        public int NextId()
        {
            return tourRepository.NextId();
        }
        public List<Tour> GetAllTours()
        {
            return tourRepository.GetAll();
        }
        public Tour GetTourById(int id)
        {
            return tourRepository.GetById(id);
        }
        public List<Tour> GetToursByStateAndGuideId(TourState state, int guideId)
        {
            return tourRepository.GetToursByStateAndGuideId(state, guideId);
        }
        public List<Tour> GetScheduledToursInDateRange(int guideId,DateTime start,DateTime end)
        {
            List<Tour> tours = new List<Tour>();
            foreach(var tour in GetToursByStateAndGuideId(TourState.Inactive, guideId))
            {
                if(tour.StartOfTheTour <= end && tour.StartOfTheTour >= end)
                {
                    tours.Add(tour);
                }
            }
            return tours;
        }
        public Tour GetTourByStateAndGuideId(TourState state, int guideId)
        {
            return tourRepository.GetTourByStateAndGuideId(state, guideId);
        }
        public List<Tour> GetToursByDateAndGuideId(DateTime date, int guideId)
        {
            return tourRepository.GetToursByDateAndGuideId(date, guideId);
        }
        public List<Tour> GetAvailableTours(DateTime date, int guideId)
        {
            return GetToursByDateAndGuideId(date, guideId).Where(t=>t.State == TourState.Inactive).ToList();
        }
        public Tour GetMostVisitedTour(int guideId, bool thisYear)
        {
            List<Tour> wantedTours = GetToursByStateAndGuideId(TourState.Finished, guideId);
            if (wantedTours.Count != 0)
            {
                if (thisYear)
                    wantedTours = wantedTours.Where(t => t.StartOfTheTour.Year == DateTime.Now.Year).ToList();
                Dictionary<int, int> numberOfGuestsOnTour = new Dictionary<int, int>();
                foreach (Tour t in wantedTours)
                {
                    numberOfGuestsOnTour.Add(t.Id, reservationService.GetNumberOfPresentGuests(t));
                }
                int mostVisitedTourId = numberOfGuestsOnTour.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                return GetTourById(mostVisitedTourId);
            }
            else
                return null;
        }
        public void Create(Tour tour)
        {
            tourRepository.Create(tour);
        }
        public void UpdateTourState(Tour tour, TourState state)
        {
            tour.State = state;
            tour.ActiveKeyPointId = (state == TourState.Active) ? tour.KeyPointIds.First() : -1;
            Update(tour);
        }
        public void UpdateActiveKeyPoint(Tour tour, int keyPointId)
        {
            tour.ActiveKeyPointId = keyPointId;
            Update(tour);
        }
        public void Remove(Tour tour)
        {
            tourRepository.Remove(tour);
        }
        public void Update(Tour tour)
        {
            tourRepository.Update(tour);
        }
        public void Subscribe(IObserver observer)
        {
            tourRepository.Subscribe(observer);
        }
        public List<Tour> SearchTours(String location, double durationStart, double durationEnd, String language, int numberGuests)
        {
            List<Tour> tours = GetAllTours();
            List<Tour> wantedTours = new List<Tour>();
            foreach (Tour tour in tours)
            {
                if (CheckSearchConditions(tour, location, durationStart, durationEnd, language, numberGuests))
                {
                    wantedTours.Add(tour);
                }
            }
            return wantedTours;
        }
        public bool CheckSearchConditions(Tour tour, string location, double durationStart, double durationEnd, string language, int numberGuests)
        {
            bool ContainsLocation, ContainsLanguage, NumberGuestsIsLower, DurationBetween;

            ContainsLocation = (location == "") ? true : (tour.Location.ToLower()).Contains(location.ToLower());
            ContainsLanguage = (language == "") ? true : (tour.Language.ToLower()).Contains(language.ToLower());
            NumberGuestsIsLower = (numberGuests == -1) ? true : numberGuests <= tour.AvailableSeats;
            DurationBetween = (durationStart == -1) ? true : durationStart <= tour.Duration && tour.Duration <= durationEnd;

            return ContainsLocation && ContainsLanguage && NumberGuestsIsLower && DurationBetween;
        }
        public List<Tour> GetAllToursWithSameLocation(Tour t)
        {
            List<Tour> wantedTours = new List<Tour>();

            foreach (Tour tour in tourRepository.GetAll())
            {
                if (tour.Location == t.Location && tour.Id != t.Id)
                {
                    wantedTours.Add(tour);
                }
            }

            return wantedTours;
        }
        //if text empty return -1
        //if text isn't double or < 0 return -2
        public double ConvertToDouble(String text)
        {
            double number;
            if (string.IsNullOrEmpty(text))
            {
                number = -1;
            }
            else if (!double.TryParse(text, out number))
            {
                MessageBox.Show("Pogresan unos! Trajanje ture mora biti realan broj!");
                return -2;
            }
            else if (number < 0)
            {
                MessageBox.Show("Pogresan unos! Trajanje ture ne moze imati negativnu vrijednost!");
                return -2;
            }
            return number;
        }
        public Tour GetFinishedTourById(int id)
        {
            foreach (Tour tour in tourRepository.GetAll())
            {
                if (tour.Id == id && tour.State == TourState.Finished)
                {
                    return tour;

                }
            }
            return null;
        }
        public List<Tour> GetToursWhichFinishedWhereGuestPresent(int guest2Id)
        {
            List<Tour> toursFinished = new List<Tour>();
            ReservationTourService reservationTourService = new ReservationTourService();
            foreach (int id in reservationTourService.FindTourIdsWhereGuestPresent(guest2Id))
            {
                Tour tour = GetFinishedTourById(id);
                if (tour != null)
                {
                    toursFinished.Add(tour);
                }
            }
            return toursFinished;
        }
        public Tour GetActivatedTourById(int id)
        {
            foreach (Tour tour in tourRepository.GetAll())
            {
                if (tour.Id == id && tour.State == TourState.Active)
                {
                    return tour;

                }
            }
            return null;
        }

        public List<Tour> GetToursWhichActivatedWhereGuestPresent(int guest2Id)
        {
            List<Tour> toursActivated = new List<Tour>();
            ReservationTourService reservationTourService = new ReservationTourService();
            foreach (int id in reservationTourService.FindTourIdsWhereGuestPresent(guest2Id))
            {
                Tour tour = GetActivatedTourById(id);
                if (tour != null)
                {
                    toursActivated.Add(tour);
                }
            }
            return toursActivated;
        }

        public Tour GetMostVisitedTourLastMonth()
        {
            List<Tour> tours = new List<Tour>();
            foreach (var tour in tourRepository.GetAll())
            {
                if (tour.State == TourState.Finished && tour.StartOfTheTour > DateTime.Now.AddMonths(-1))
                {
                    tours.Add(tour);
                }
            }
            List<Tour> sortedTours = tours.OrderByDescending(t => t.MaxNumberGuests - t.AvailableSeats).ToList();
            return sortedTours.First();
        }
    }
}
