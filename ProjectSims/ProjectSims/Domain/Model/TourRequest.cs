﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectSims.Serializer;

namespace ProjectSims.Domain.Model
{
    public enum TourRequestState { Waiting, Accepted, Invalid }
    public class TourRequest : ISerializable
    {
        public int Id { get; set; }
        public int Guest2Id { get; set; }
        public Guest2 Guest2 { get; set; }
        public int GuideId { get; set; }
        public Guide Guide { get; set; }
        public TourRequestState State { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int MaxNumberGuests { get; set; }
        public DateOnly DateRangeStart { get; set; }
        public DateOnly DateRangeEnd { get; set; }
        public DateTime CreationDate { get; set; }
        public bool RequestForComplexTour { get; set; }
        public DateTime AcceptedStartOfAppointment { get; set; }
        public DateTime AcceptedEndOfAppointment { get; set; }
        public TourRequest()
        {
        }
        public TourRequest(int guest2Id, int guideId, TourRequestState state, string location, string description, string language, int maxNumberGuests, DateOnly dateRangeStart, DateOnly dateRangeEnd,bool requestForComplexTour)
        {
            Guest2Id = guest2Id;
            GuideId = guideId;
            State = state;
            Location = location;
            Description = description;
            Language = language;
            MaxNumberGuests = maxNumberGuests;
            DateRangeStart = dateRangeStart;
            DateRangeEnd = dateRangeEnd;
            CreationDate = DateTime.Now;
            RequestForComplexTour = requestForComplexTour;
            AcceptedStartOfAppointment = new DateTime(0001, 1, 1);
            AcceptedEndOfAppointment = new DateTime(0001, 1, 1);

        }

        public static TourRequestState GetRequestState(string state)
        {
            return state switch
            {
                "na cekanju" => TourRequestState.Waiting ,
                "prihvacen" => TourRequestState.Accepted,
                _ => TourRequestState.Invalid
            };
        }

        public static string GetRequestState(TourRequestState state)
        {
            return state switch
            {
                TourRequestState.Waiting => "na cekanju",
                TourRequestState.Accepted => "prihvacen",
                _ => "nevazeci"
            };
        }
        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Guest2Id = Convert.ToInt32(values[1]);
            GuideId = Convert.ToInt32(values[2]);
            State = (TourRequestState)Enum.Parse(typeof(TourRequestState), values[3]);
            Location = values[4];
            Description = values[5];
            Language = values[6];
            MaxNumberGuests = Convert.ToInt32(values[7]);
            DateRangeStart = DateOnly.ParseExact(values[8], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateRangeEnd = DateOnly.ParseExact(values[9], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            CreationDate = DateTime.ParseExact(values[10], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            RequestForComplexTour = Convert.ToBoolean(values[11]);
            AcceptedStartOfAppointment = DateTime.ParseExact(values[12], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            AcceptedEndOfAppointment = DateTime.ParseExact(values[13], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        public string[] ToCSV()
        {
            string[] csvvalues = { Id.ToString(), Guest2Id.ToString(), GuideId.ToString(), State.ToString(), Location, Description, Language, MaxNumberGuests.ToString(), DateRangeStart.ToString("dd/MM/yyyy"), DateRangeEnd.ToString("dd/MM/yyyy"),CreationDate.ToString("dd/MM/yyyy"),RequestForComplexTour.ToString(), AcceptedStartOfAppointment.ToString("dd/MM/yyyy HH:mm:ss") , AcceptedEndOfAppointment.ToString("dd/MM/yyyy HH:mm:ss") };
            return csvvalues;
        }
    }
}
