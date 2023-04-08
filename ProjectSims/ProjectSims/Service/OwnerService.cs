﻿using ProjectSims.Domain.Model;
using ProjectSims.Repository;
using ProjectSims.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSims.Service
{
    public class OwnerService
    {
        private OwnerRepository owners;
        private RequestService requestService;

        public OwnerService()
        {
            owners = new OwnerRepository();
            requestService = new RequestService();
        }

        public List<Owner> GetAllOwners()
        {
            return owners.GetAll();
        }

        public bool HasWaitingRequests(int ownerId)
        {
            foreach(Request request in requestService.GetAllRequestByOwner(ownerId))
            {
                if (request.State.Equals(RequestState.Waiting))
                {
                    return true;
                }
            }
            return false;
        }

        public void Create(Owner owner)
        {
            owners.Add(owner);
        }

        public void Delete(Owner owner)
        {
            owners.Remove(owner);
        }

        public void Update(Owner owner)
        {
            owners.Update(owner);
        }

        public void Subscribe(IObserver observer)
        {
            owners.Subscribe(observer);
        }
    }
}
