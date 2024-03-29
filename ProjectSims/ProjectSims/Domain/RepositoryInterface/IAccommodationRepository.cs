﻿using ProjectSims.Domain.Model;
using ProjectSims.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSims.Domain.RepositoryInterface
{
    public interface IAccommodationRepository : IGenericRepository<Accommodation, int>, ISubject
    {
        public List<Accommodation> GetAllByOwner(int ownerId);
        public List<Accommodation> GetAllByLocation(int locationId);
    }
}
