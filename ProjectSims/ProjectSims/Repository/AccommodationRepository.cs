﻿using ProjectSims.FileHandler;
using ProjectSims.Domain.Model;
using ProjectSims.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectSims.Domain.RepositoryInterface;

namespace ProjectSims.Repository
{
    class AccommodationRepository : IAccommodationRepository
    {
        private AccommodationFileHandler accommodationFileHandler;
        private LocationFileHandler locationFileHandler;
        private AccommodationScheduleFileHandler accommodationScheduleFileHandler;
        private List<Accommodation> accommodations;
        private List<Location> locations;
        private List<AccommodationSchedule> schedules;
        private readonly OwnerFileHandler _ownerFileHandler;
        private readonly List<Owner> _owners;
        private List<IObserver> observers;

        public AccommodationRepository()
        {
            accommodationFileHandler = new AccommodationFileHandler();
            accommodations = accommodationFileHandler.Load();
            locationFileHandler = new LocationFileHandler();
            locations = locationFileHandler.Load();
            accommodationScheduleFileHandler = new AccommodationScheduleFileHandler();
            schedules = accommodationScheduleFileHandler.Load();
            observers = new List<IObserver>();
        }

        public Accommodation GetById(int id)
        {
            return accommodations.Find(a => a.Id == id);
        }

        public int NextId()
        {
            return accommodations.Max(a => a.Id) + 1;
        }

        public void Create(Accommodation accommodation)
        {
            accommodation.Id = NextId();
            accommodations.Add(accommodation); 
            accommodationFileHandler.Save(accommodations);
            NotifyObservers();
        }

        public void Remove(Accommodation accommodation)
        {
            accommodations.Remove(accommodation);
            accommodationFileHandler.Save(accommodations);
            NotifyObservers();
        }

        public void Update(Accommodation accommodation)
        {
            int index = accommodations.FindIndex(a => accommodation.Id == a.Id);
            if (index != -1)
            {
                accommodations[index] = accommodation;
            }
            accommodationFileHandler.Save(accommodations);
            NotifyObservers();
        }

        public List<Accommodation> GetAll()
        {
            return accommodations;
        }

        public List<Accommodation> GetAllByOwner(int ownerId)
        {
            List<Accommodation> ownerAccomodations = new List<Accommodation>();
            foreach (Accommodation accommodation in GetAll())
            {
                if (accommodation.IdOwner == ownerId)
                {
                    ownerAccomodations.Add(accommodation);
                }
            }
            return ownerAccomodations;
        }
        public List<Accommodation> GetAllByLocation(int locationId)
        {
            List<Accommodation> accomodations = new List<Accommodation>();
            foreach (Accommodation accommodation in GetAll())
            {
                if (accommodation.IdLocation == locationId)
                {
                    accomodations.Add(accommodation);
                }
            }
            return accomodations;
        }


        public void Subscribe(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }
    }   
}