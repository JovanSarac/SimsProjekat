﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectSims.Domain.RepositoryInterface;
using ProjectSims.Repository;
using ProjectSims.Serializer;
using ProjectSims.Service;
using ProjectSims.WPF.View.Guest1View;

namespace ProjectSims.Domain.Model
{
    public enum ReservationState { Active, Canceled }
    public class AccommodationReservation : ISerializable
    {
        public int Id { get; set; }
        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; }
        public int GuestId { get; set; }
        public Guest1 Guest { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int GuestNumber { get; set; }
        public ReservationState State { get; set; }
        public bool RatedAccommodation { get; set; }
        public bool RatedGuest { get; set; }

        public AccommodationReservation() { }

        public AccommodationReservation(int id, int accommodationId, int guestId,  DateOnly checkInDate, DateOnly checkOutDate, int guestNumber, ReservationState state, bool ratedAccommodation, bool ratedGuest, Accommodation accommodation)
        {
            Id = id;
            AccommodationId = accommodationId;
            GuestId = guestId;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            GuestNumber = guestNumber;
            State = state;
            RatedAccommodation = ratedAccommodation;
            RatedGuest = ratedGuest;
            Accommodation = accommodation;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            AccommodationId = Convert.ToInt32(values[1]);
            GuestId = Convert.ToInt32(values[2]);
            CheckInDate = DateOnly.ParseExact(values[3], "dd.MM.yyyy");
            CheckOutDate = DateOnly.ParseExact(values[4], "dd.MM.yyyy");
            GuestNumber = Convert.ToInt32(values[5]);
            State = Enum.Parse<ReservationState>(values[6]);
            RatedAccommodation = Convert.ToBoolean(values[7]);
            RatedGuest = Convert.ToBoolean(values[8]);
        }

        public string[] ToCSV()
        {
            string[] csvvalues = {
                Id.ToString(),
                AccommodationId.ToString(),
                GuestId.ToString(),
                CheckInDate.ToString("dd.MM.yyyy"),
                CheckOutDate.ToString("dd.MM.yyyy"),
                GuestNumber.ToString(),  
                State.ToString(),
                RatedAccommodation.ToString(),
                RatedGuest.ToString()
            };
            return csvvalues;
        }
    }
}
