﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectSims.Domain.Model;

namespace ProjectSims.Domain.RepositoryInterface
{
    public interface IAccommodationRatingRepository : IGenericRepository<AccommodationAndOwnerRating, int>
    {
        public AccommodationAndOwnerRating GetByReservationId(int reservationId);
        public List<AccommodationAndOwnerRating> GetAllByGuestId(int guestId);
    }
}