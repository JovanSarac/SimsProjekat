﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectSims.Domain.Model;
using ProjectSims.Repository;
using ProjectSims.Observer;
using ProjectSims.FileHandler;
using ProjectSims.WPF.View.Guest1View;
using ProjectSims.Domain.RepositoryInterface;
using ProjectSims.WPF.View.Guest1View.MainPages;
using ProjectSims.WPF.View.OwnerView.Pages;

namespace ProjectSims.Service
{
    public class AccommodationReservationService
    {
        private IAccommodationReservationRepository reservationRepository;
        private IRequestRepository requestRepository;
        private IGuest1Repository guest1Repository;
        private IAccommodationRepository accommodationRepository;
        private IAccommodationScheduleRepository accommodationScheduleRepository;
        private IRenovationRecommendationRepository renovationRecommendationRepository;
        private IAccommodationRatingRepository accommodationRatingRepository;

        public AccommodationReservationService()
        {
            reservationRepository = Injector.CreateInstance<IAccommodationReservationRepository>();
            requestRepository = Injector.CreateInstance<IRequestRepository>();
            guest1Repository = Injector.CreateInstance<IGuest1Repository>();
            accommodationRepository = Injector.CreateInstance<IAccommodationRepository>();
            accommodationScheduleRepository = Injector.CreateInstance<IAccommodationScheduleRepository>();
            renovationRecommendationRepository = Injector.CreateInstance<IRenovationRecommendationRepository>();
            accommodationRatingRepository = Injector.CreateInstance<IAccommodationRatingRepository>();

            InitializeGuest();
            InitializeAccommodation();
        }

        private void InitializeGuest()
        {
            foreach(var reservation in reservationRepository.GetAll())
            {
                reservation.Guest = guest1Repository.GetById(reservation.GuestId);
            }
        }

        private void InitializeAccommodation()
        {
            foreach (var reservation in reservationRepository.GetAll())
            {
                reservation.Accommodation = accommodationRepository.GetById(reservation.AccommodationId);
            }
        }
        public List<AccommodationReservation> GetAllReservations()
        {
            return reservationRepository.GetAll();
        }

        public List<AccommodationReservation> GetAllByOwnerId(int id)
        {
            List<AccommodationReservation> reservations = new List<AccommodationReservation>();
            foreach (var reservation in GetAllReservations())
            {
                if (reservation.Accommodation.IdOwner == id && reservation.State == ReservationState.Active)
                {
                    reservations.Add(reservation);
                }
            }
            return reservations;
        }  
        
        public List<AccommodationReservation> GetActiveAndCanceledByOwnerId(int ownerId, int accommodationId)
        {
            List<AccommodationReservation> reservations = new List<AccommodationReservation>();
            foreach (var reservation in GetAllReservations())
            {
                if (reservation.Accommodation.IdOwner == ownerId && reservation.AccommodationId == accommodationId)
                {
                    reservations.Add(reservation);
                }
            }
            return reservations;
        }

        public int GetAllReservationsByYear(List<AccommodationReservation> reservations, string year)
        {
            int number = 0;
            foreach (AccommodationReservation reservation in reservations)
            {
                if (reservation.CheckInDate.Year.ToString().Equals(year))
                {
                    number++;
                }
            }
            return number;
        }

        public int GetAllReservationsByMonth(List<AccommodationReservation> reservations, string month, string year)
        {
            int number = 0;
            foreach (AccommodationReservation reservation in reservations)
            {
                if (reservation.CheckInDate.Month.ToString().Equals(month) && reservation.CheckInDate.Year.ToString().Equals(year))
                {
                    number++;
                }
            }
            return number;
        }

        public int GetAllShiftedReservationsByYear(List<AccommodationReservation> reservations, string year)
        {
            int number = 0;
            foreach (AccommodationReservation reservation in reservations)
            {
                foreach (Request request in requestRepository.GetAllByOwner(reservation.Accommodation.IdOwner))
                {
                    if (reservation.CheckInDate.Year.ToString().Equals(year) && reservation.Id == request.ReservationId)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int GetAllShiftedReservationsByMonth(List<AccommodationReservation> reservations, string month, string year)
        {
            int number = 0;
            foreach (AccommodationReservation reservation in reservations)
            {
                foreach (Request request in requestRepository.GetAllByOwner(reservation.Accommodation.IdOwner))
                {
                    if (reservation.CheckInDate.Year.ToString().Equals(year) && reservation.CheckInDate.Month.ToString().Equals(month) && reservation.Id == request.ReservationId)
                    {
                        number++;
                    }
                }
            }
            return number;
        }
        
        public int GetAllCanceledReservationsByYear(List<AccommodationReservation> reservations, string year)
        {
            int number = 0;
            foreach (AccommodationReservation reservation in reservations)
            {
                if (reservation.CheckInDate.Year.ToString().Equals(year) && reservation.State == ReservationState.Canceled)
                {
                    number++;
                }
            }
            return number;
        }
        
        public int GetAllCanceledReservationsByMonth(List<AccommodationReservation> reservations, string month, string year)
        {
            int number = 0;
            foreach (AccommodationReservation reservation in reservations)
            {
                if (reservation.CheckInDate.Year.ToString().Equals(year) && reservation.CheckInDate.Month.ToString().Equals(month) && reservation.State == ReservationState.Canceled)
                {
                    number++;
                }
            }
            return number;
        }
        
        public int GetAllRenovationReccommendationsByYear(string year, int ownerId)
        {
            int number = 0;
            foreach (AccommodationAndOwnerRating rating in accommodationRatingRepository.GetAllByOwnerId(ownerId))
            {
                foreach (RenovationRecommendation renovation in renovationRecommendationRepository.GetAll())
                {
                    if (rating.Reservation.CheckInDate.Year.ToString().Equals(year) && rating.RenovationId == renovation.Id)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int GetAllRenovationReccommendationsByMonth(string month, string year, int ownerId)
        {
            int number = 0;
            foreach (AccommodationAndOwnerRating rating in accommodationRatingRepository.GetAllByOwnerId(ownerId))
            {
                foreach (RenovationRecommendation renovation in renovationRecommendationRepository.GetAll())
                {
                    if (rating.Reservation.CheckInDate.Year.ToString().Equals(year) && rating.Reservation.CheckInDate.Month.ToString().Equals(month) && rating.RenovationId == renovation.Id)
                    {
                        number++;
                    }
                }
            }
            return number;
        }
        
        public AccommodationReservation GetReservation(int id)
        {
            return reservationRepository.GetById(id);
        }

        public List<AccommodationReservation> GetReservationByGuest(int guestId)
        {
            return reservationRepository.GetByGuest(guestId);
        }
        
        public AccommodationReservation GetReservation(int guestId, int accommodationId, DateOnly checkInDate, DateOnly checkOutDate)
        {
            return reservationRepository.GetReservation(guestId, accommodationId, checkInDate, checkOutDate);
        }

        public void CreateReservation(int accommodationId, int guestId, DateOnly checkIn, DateOnly checkOut, int guestNumber)
        {
            int id = reservationRepository.NextId();
            AccommodationReservation reservation = new AccommodationReservation(id, accommodationId, guestId, checkIn, checkOut, guestNumber, ReservationState.Active, false, false, accommodationRepository.GetById(accommodationId));
            reservationRepository.Create(reservation);

            AccommodationSchedule schedule = accommodationRepository.GetById(accommodationId).Schedule;
            DateRanges dateRange = new DateRanges(checkIn, checkOut);
            accommodationScheduleRepository.AddUnavailableDate(schedule, dateRange);
            accommodationScheduleRepository.Update(schedule);
        }

        public void Update(AccommodationReservation reservation)
        {
            reservationRepository.Update(reservation);
        }

        public void RemoveReservation(AccommodationReservation reservation)
        {
            reservation.State = ReservationState.Canceled;
            reservationRepository.Update(reservation);

            UpdateRequestsWhenCancelReservation(reservation);         
        }

        public void UpdateRequestsWhenCancelReservation(AccommodationReservation reservation)
        {
            Request request = requestRepository.GetByReservationId(reservation.Id);
            requestRepository.Remove(request);
        }

        public void CloseAccommodation(Accommodation selectedAccommodation)
        {
            Accommodation accommodationToRemove = accommodationRepository.GetAllByOwner(selectedAccommodation.IdOwner).Find(accommodation => accommodation.Id == selectedAccommodation.Id);
            accommodationRepository.Remove(accommodationToRemove);
        }

        public bool CanCancel(AccommodationReservation reservation)
        {
            int dismissalDays = reservation.Accommodation.DismissalDays;
            if(DateOnly.FromDateTime(DateTime.Today) <= reservation.CheckInDate.AddDays(-dismissalDays))
            {
                return true;
            }
            return false;
        }
        public List<AccommodationReservation> GetAccommodationsForRating(Guest1 guest)
        {
            List<AccommodationReservation> guestReservations = new List<AccommodationReservation>();
            List<AccommodationReservation> accommodationsForRating = new List<AccommodationReservation>();

            guestReservations = GetReservationByGuest(guest.Id);
            foreach (AccommodationReservation reservation in guestReservations)
            {
                if(DateOnly.FromDateTime(DateTime.Today) <= reservation.CheckOutDate.AddDays(5) && DateOnly.FromDateTime(DateTime.Today) >= reservation.CheckOutDate && reservation.RatedAccommodation == false)
                {
                    accommodationsForRating.Add(reservation);
                }
            }
            return accommodationsForRating;
        }

        public void ChangeReservationRatedState(AccommodationReservation reservation)
        {
            reservation.RatedAccommodation = true;
            reservationRepository.Update(reservation);
        }

        public Boolean IsAnyGuestRatable()
        {
            List<AccommodationReservation> reservations = GetAllReservations();

            foreach (var item in reservations)
            {
                if (DateOnly.FromDateTime(DateTime.Today).CompareTo(item.CheckOutDate) > 0 && GetReservation(item.Id).RatedGuest == false)
                {
                    return true;
                }
            }
            return false;
        }

        public void Subscribe(IObserver observer)
        {
            reservationRepository.Subscribe(observer);
        }
    }
}
