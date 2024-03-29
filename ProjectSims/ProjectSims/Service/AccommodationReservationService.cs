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
using ceTe.DynamicPDF.PageElements.Charting;
using LiveCharts;

namespace ProjectSims.Service
{
    public class AccommodationReservationService
    {
        private IAccommodationReservationRepository reservationRepository;
        private IRequestRepository requestRepository;
        private IGuest1Repository guest1Repository;
        private IAccommodationRepository accommodationRepository;
        private IAccommodationScheduleRepository accommodationScheduleRepository;
        private ISuperGuestRepository superGuestRepository;

        public AccommodationReservationService()
        {
            reservationRepository = Injector.CreateInstance<IAccommodationReservationRepository>();
            requestRepository = Injector.CreateInstance<IRequestRepository>();
            guest1Repository = Injector.CreateInstance<IGuest1Repository>();
            accommodationRepository = Injector.CreateInstance<IAccommodationRepository>();
            accommodationScheduleRepository = Injector.CreateInstance<IAccommodationScheduleRepository>();
            superGuestRepository = Injector.CreateInstance<ISuperGuestRepository>();

            InitializeGuest();
            InitializeAccommodation();
        }

        private void InitializeGuest()
        {
            foreach (var reservation in reservationRepository.GetAll())
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
        public List<AccommodationReservation> GetActiveReservationsByGuest(int guestId)
        {
            return reservationRepository.GetAllActiveByGuest(guestId);
        }
        public List<AccommodationReservation> GetCanceledReservationsByGuest(int guestId)
        {
            return reservationRepository.GetAllCanceledByGuest(guestId);
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

        public List<AccommodationReservation> GetAllReservationsByAccommodationId(int accommodationId)
        {
            List<AccommodationReservation> reservations = new List<AccommodationReservation>();
            foreach (var reservation in GetAllReservations())
            {
                if (reservation.AccommodationId == accommodationId)
                {
                    reservations.Add(reservation);
                }
            }
            return reservations;
        }

        public int CountAllReservationsByYear(List<AccommodationReservation> reservations, string year)
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

        public List<AccommodationReservation> GettAllReservationsByYear(List<AccommodationReservation> reservations, string year)
        {
            List<AccommodationReservation> filteredReservations = new List<AccommodationReservation>();
            foreach (AccommodationReservation reservation in reservations)
            {
                if (reservation.CheckInDate.Year.ToString().Equals(year))
                {
                    filteredReservations.Add(reservation);
                }
            }
            return filteredReservations;
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

        public AccommodationReservation GetReservation(int id)
        {
            return reservationRepository.GetById(id);
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

            TakeBonusPointIfSuperGuest(guestId);
        }

        public void TakeBonusPointIfSuperGuest(int guestId)
        {
            Guest1 guest = guest1Repository.GetById(guestId);
            if (guest.SuperGuestId != -1 && guest.SuperGuest.BonusPoints > 0)
            {
                SuperGuest superGuest = guest.SuperGuest;
                superGuest.BonusPoints--;
                superGuestRepository.Update(superGuest);
            }
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

        public bool CanCancel(AccommodationReservation reservation)
        {
            int dismissalDays = reservation.Accommodation.DismissalDays;
            if (DateOnly.FromDateTime(DateTime.Today) <= reservation.CheckInDate.AddDays(-dismissalDays))
            {
                return true;
            }
            return false;
        }
        public List<AccommodationReservation> GetAccommodationsForRating(Guest1 guest)
        {
            List<AccommodationReservation> guestReservations = new List<AccommodationReservation>();
            List<AccommodationReservation> accommodationsForRating = new List<AccommodationReservation>();

            guestReservations = GetActiveReservationsByGuest(guest.Id);
            foreach (AccommodationReservation reservation in guestReservations)
            {
                if (DateOnly.FromDateTime(DateTime.Today) <= reservation.CheckOutDate.AddDays(5) && DateOnly.FromDateTime(DateTime.Today) >= reservation.CheckOutDate && reservation.RatedAccommodation == false)
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

        public List<AccommodationReservation> IsAnyGuestRatable(int ownerId)
        {
            List<AccommodationReservation> reservations = new List<AccommodationReservation>();
            foreach (var item in GetAllByOwnerId(ownerId))
            {
                if (DateOnly.FromDateTime(DateTime.Today).CompareTo(item.CheckOutDate) >= 0
                    && item.CheckOutDate.AddDays(5).CompareTo(DateOnly.FromDateTime(DateTime.Today)) >= 0
                    && GetReservation(item.Id).RatedGuest == false)
                {
                    reservations.Add(item);
                }
            }
            return reservations;
        }

        public void Subscribe(IObserver observer)
        {
            reservationRepository.Subscribe(observer);
        }

        public int FindMaxCountAccommodation(Owner owner, string[] YearLabels)
        {
            Dictionary<int, double[]> allBusyness = FindStatisticsForAllAccommodations(owner, YearLabels);
            KeyValuePair<int, double[]> max = allBusyness.FirstOrDefault();
            foreach (var item in allBusyness)
            {
                if (item.Value[1] > max.Value[1])
                {
                    max = item;
                }
            }
            return max.Key;
        }

        public int FindMinCountAccommodation(Owner owner, string[] YearLabels)
        {
            Dictionary<int, double[]> allBusyness = FindStatisticsForAllAccommodations(owner, YearLabels);
            KeyValuePair<int, double[]> min = allBusyness.FirstOrDefault();
            foreach (var item in allBusyness)
            {
                if (item.Value[1] < min.Value[1])
                {
                    min = item;
                }
            }
            return min.Key;
        }

        public int FindMostBusyAccommodation(Owner owner, string[] YearLabels)
        {
            Dictionary<int, double[]> allBusyness = FindStatisticsForAllAccommodations(owner, YearLabels);
            KeyValuePair<int, double[]> most = allBusyness.FirstOrDefault();
            foreach (var item in allBusyness)
            {
                if (item.Value[0] > most.Value[0])
                {
                    most = item;
                }
            }
            return most.Key;
        }

        public int FindLeastBusyAccommodation(Owner owner, string[] YearLabels)
        {
            Dictionary<int, double[]> allBusyness = FindStatisticsForAllAccommodations(owner, YearLabels);
            KeyValuePair<int, double[]> least = allBusyness.First();
            foreach (var item in allBusyness)
            {
                if (item.Value[0] < least.Value[0])
                {
                    least = item;
                }
            }
            return least.Key;
        }

        public Dictionary<int, double[]> FindStatisticsForAllAccommodations(Owner owner, string[] YearLabels)
        {
            Dictionary<int, double[]> accommodationsStatistics = new Dictionary<int, double[]>();
            List<Accommodation> accommodations = accommodationRepository.GetAllByOwner(owner.Id);
            foreach (Accommodation accommodation in accommodations)
            {
                List<AccommodationReservation> reservations = GetAllReservationsByAccommodationId(accommodation.Id);
                accommodationsStatistics.Add(accommodation.Id, SumBusynessAndReservationCountForEachYear(reservations, YearLabels));
            }
            return accommodationsStatistics;
        }

        public double[] SumBusynessAndReservationCountForEachYear(List<AccommodationReservation> reservations, string[] YearLabels)
        {
            double[] sumAndCount = new double[] { 0, 0 };
            Dictionary<string, double[]> busyness = CountBusynessAndReservationCountForEachYear(reservations, YearLabels);
            foreach (var item in busyness)
            {
                sumAndCount[0] += item.Value[0];
                sumAndCount[1] += item.Value[1];
            }
            return sumAndCount;
        }
        public int DisplayMostVisitedMonth(ChartValues<int> TotalMonthReservations, int MostVisitedMonth)
        {
            double[] daysInMonths = new double[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            MostVisitedMonth = 0;
            double unavailability = TotalMonthReservations[0] / daysInMonths[0];
            for (int i = 1; i < TotalMonthReservations.Count(); i++)
            {
                if (TotalMonthReservations[i] / daysInMonths[i] > unavailability)
                {
                    MostVisitedMonth = i;
                    unavailability = TotalMonthReservations[i] / daysInMonths[i];
                }
            }
            MostVisitedMonth += 1;
            return MostVisitedMonth;
        }

        public string FindMostVisitedYear(string[] YearLabels, List<AccommodationReservation> Reservations)
        {
            string MostVisitedYear = "";
            Dictionary<string, double[]> busynessThroughYears = CountBusynessAndReservationCountForEachYear(Reservations, YearLabels);
            KeyValuePair<string, double[]> mostVisited = busynessThroughYears.FirstOrDefault();
            MostVisitedYear = mostVisited.Key;
            foreach (var item in busynessThroughYears)
            {
                if (item.Value[0] > mostVisited.Value[0])
                {
                    mostVisited = item;
                    MostVisitedYear = mostVisited.Key;
                }
            }
            return MostVisitedYear;
        }
        public Dictionary<string, double[]> CountBusynessAndReservationCountForEachYear(List<AccommodationReservation> reservations, string[] YearLabels)
        {
            Dictionary<string, double[]> bussynessAndReservationCountInYears = new Dictionary<string, double[]>();
            foreach (var year in YearLabels)
            {
                bussynessAndReservationCountInYears.Add(year, new double[] { CountBusynessAndReservationCountInOneYear(reservations, year)[0],
                                                                             CountBusynessAndReservationCountInOneYear(reservations, year)[1] });
            }
            return bussynessAndReservationCountInYears;
        }

        public double[] CountBusynessAndReservationCountInOneYear(List<AccommodationReservation> reservations, string year)
        {
            double[] busyness = new double[2] { 0, 0 };
            foreach (var item in GettAllReservationsByYear(reservations, year))
            {
                busyness[0] += (item.CheckOutDate.DayNumber - item.CheckInDate.DayNumber);
                busyness[1]++;
            }
            busyness[0] = busyness[0] / 365.0;
            return busyness;
        }
    }
}

