﻿using ProjectSims.Domain.Model;
using ProjectSims.Repository;
using ProjectSims.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;
using ProjectSims.Domain.RepositoryInterface;
using ProjectSims.WPF.View.Guest1View.MainPages;

namespace ProjectSims.Service
{
    public class ReservationTourService
    {
        private IReservationTourRepository reservationTourRepository;
        private IGuest2Repository guest2Repository;
        private ITourRepository tourRepository;
        private IKeyPointRepository keyPointRepository;


        public ReservationTourService()
        {
            reservationTourRepository = Injector.CreateInstance<IReservationTourRepository>();
            guest2Repository = Injector.CreateInstance<IGuest2Repository>();
            tourRepository = Injector.CreateInstance<ITourRepository>();
            keyPointRepository = Injector.CreateInstance<IKeyPointRepository>();
            InitializeGuest();
            InitializeTour();
            InitializeKeyPoint();
        }
        private void InitializeGuest()
        {
            foreach (var item in reservationTourRepository.GetAll())
            {
                item.Guest2 = guest2Repository.GetById(item.Guest2Id);
            }
        }
        private void InitializeTour()
        {
            foreach (var item in reservationTourRepository.GetAll())
            {
                item.Tour = tourRepository.GetById(item.TourId);
            }
        }
        private void InitializeKeyPoint()
        {
            foreach (var item in reservationTourRepository.GetAll())
            {
                item.KeyPointWhereGuestArrived = keyPointRepository.GetById(item.KeyPointWhereGuestArrivedId);
            }
        }
        public List<ReservationTour> GetAllReservations()
        {
            return reservationTourRepository.GetAll();
        }
        public List<ReservationTour> GetReservationsByTour(Tour tour)
        {
            return reservationTourRepository.GetReservationsByTour(tour);
        }
        public List<Guest2> GetGuestsForScheduledToursByGuideId(int guideId)
        {
            List<Tour> scheduledTours = tourRepository.GetToursByStateAndGuideId(TourState.Inactive,guideId);
            List<ReservationTour> reservations = new List<ReservationTour>();
            foreach (var scheduledTour in scheduledTours)
            {
                reservations.AddRange(reservationTourRepository.GetReservationsByTour(scheduledTour));
            }
            List<Guest2> loadedGuests = reservations.Select(r=>r.Guest2).ToList();
            return loadedGuests.Distinct().ToList();
        }
        public List<ReservationTour> GetReservationsByTourAndState(Tour tour,Guest2State state)
        {
            return reservationTourRepository.GetReservationsByTourAndState(tour, state);
        }
        public List<int> GetGuestIdsByTourAndState(Tour tour, Guest2State state)
        {
            return GetReservationsByTourAndState(tour, state).Select(r=> r.Guest2Id).ToList();
        }
        public ReservationTour GetReservationByGuestAndTour(Tour tour, Guest2 guest)
        {
            return reservationTourRepository.GetReservationByGuestAndTour(tour, guest);
        }
        public int GetNumberOfPresentGuests(Tour tour)
        {
            int numberOfPresentGuests = 0;
            List<ReservationTour> wantedReservations = GetReservationsByTourAndState(tour, Guest2State.Present);
            wantedReservations.ForEach(r => numberOfPresentGuests += r.NumberGuest);
            return numberOfPresentGuests;
        }
        public int GetNumberOfPresentGuestsWithVoucher(Tour tour)
        {
            int numberOfPresentGuestsWithVoucher = 0;
            List<ReservationTour> wantedReservations = GetReservationsByTourAndState(tour, Guest2State.Present);
            wantedReservations.ForEach(r => { if (r.UsedVoucher) numberOfPresentGuestsWithVoucher += r.NumberGuest; });
            return numberOfPresentGuestsWithVoucher;
        }
        public double GetPercentageOfPresentGuestsWithVoucher(Tour tour)
        {
            double numberOfPresentGuestsWithVoucher = (double)GetNumberOfPresentGuestsWithVoucher(tour);
            double numberOfPresentGuests = (double)GetNumberOfPresentGuests(tour);
            if (numberOfPresentGuestsWithVoucher != 0)
            {
                double percentage = (numberOfPresentGuestsWithVoucher / numberOfPresentGuests) *100;
                return Math.Round(percentage, 2);
            }
            else
                return 0;
        }
        public int GetNumberOfPresentGuestsByAgeLimit(Tour tour,int lowerLimit)
        {
            int numberOfPresentGuestsThatAge = 0;
            List<ReservationTour> wantedReservations = GetReservationsByTourAndState(tour, Guest2State.Present);
            if (lowerLimit == 0)
                wantedReservations.ForEach(r => { if (r.GuestAgeOnTour <18 ) numberOfPresentGuestsThatAge += r.NumberGuest; });
            else if(lowerLimit == 18)
                wantedReservations.ForEach(r => { if (r.GuestAgeOnTour >= 18 && r.GuestAgeOnTour <=50 ) numberOfPresentGuestsThatAge += r.NumberGuest; });
            else
                wantedReservations.ForEach(r => { if (r.GuestAgeOnTour > 50) numberOfPresentGuestsThatAge += r.NumberGuest; });
            return numberOfPresentGuestsThatAge;
        }
        public void Create(ReservationTour reservation)
        {
            reservationTourRepository.Create(reservation);
        }
        public void Remove(ReservationTour reservation)
        {
            reservationTourRepository.Remove(reservation);
        }  
        public void UpdateGuestsState(Tour tour,Guest2State state)
        {
            List<ReservationTour> tourReservations = GetReservationsByTour(tour);
            tourReservations.ForEach(r => r.State = state);
            tourReservations.ForEach(r => Update(r));
        }
        public void UpdateGuestState(int guestId,Tour tour,Guest2State state)
        {
            ReservationTour reservation = GetReservationsByTour(tour).Find(r=>r.Guest2Id == guestId);
            reservation.State = state;
            if(state == Guest2State.Present)
                reservation.KeyPointWhereGuestArrivedId = tour.ActiveKeyPointId;
            Update(reservation);
        }
        public ReservationTour GetTourIdWhereGuestIsWaiting(Guest2 guest)
        {
            return reservationTourRepository.GetTourIdWhereGuestIsWaiting(guest);
        }
        public List<int> FindTourIdsWhereGuestPresent(int guestId)
        {
            List<int> tourIds = new List<int>();
            foreach (ReservationTour reservation in GetAllReservations())
            {
                if(reservation.Guest2Id == guestId && reservation.State == Guest2State.Present && reservation.RatedTour != true)

                {
                    tourIds.Add(reservation.TourId);
                }
            }
            return tourIds;
        }
        public void Update(ReservationTour reserevation)
        {
            reservationTourRepository.Update(reserevation);
        }
        public void Subscribe(IObserver observer)
        {
            reservationTourRepository.Subscribe(observer);
        }
        public List<ReservationTour> GetReservationsForGuest(int guest2Id)
        {
            return GetAllReservations().Where(r => r.Guest2Id == guest2Id).ToList();
        }
    }
}
