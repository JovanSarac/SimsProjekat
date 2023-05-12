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
    class Guest1Repository : IGuest1Repository
    {
        private Guest1FileHandler guestFileHandler;
        private List<Guest1> guests;
        private List<IObserver> observers;

        public Guest1Repository()
        {
            guestFileHandler = new Guest1FileHandler();
            guests = guestFileHandler.Load();
            observers = new List<IObserver>();
        }
        public List<Guest1> GetAll()
        {
            return guests;
        }

        public int NextId()
        {
            return guests.Max(t => t.Id) + 1;
        }

        public void Create(Guest1 entity)
        {
            entity.Id = NextId();
            guests.Add(entity);
            guestFileHandler.Save(guests);
            NotifyObservers();
        }
        public void Update(Guest1 entity)
        {
            int index = guests.FindIndex(guest => guest.Id == guest.Id);
            if (index != -1)
            {
                guests[index] = entity;
            }
            guestFileHandler.Save(guests);
            NotifyObservers();
        }

        public void Remove(Guest1 entity)
        { 
            guests.Remove(entity);
            guestFileHandler.Save(guests);
            NotifyObservers();
        }
        public Guest1 GetById(int key)
        {
            return guests.Find(guest => guest.Id == key);
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