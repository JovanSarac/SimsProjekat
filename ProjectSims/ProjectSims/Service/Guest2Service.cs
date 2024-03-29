﻿using ProjectSims.Domain.Model;
using ProjectSims.Repository;
using ProjectSims.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectSims.Domain.RepositoryInterface;
using ProjectSims.Serializer;

namespace ProjectSims.Service
{
    public class Guest2Service
    {
       private IGuest2Repository guest2Repository;
       private VoucherService voucherService;
        private IUserRepository userRepository;

        public Guest2Service()
        {
            guest2Repository = Injector.CreateInstance<IGuest2Repository>();
            userRepository = Injector.CreateInstance<IUserRepository>();
            voucherService = new VoucherService();
            InitializeUser();
        }
        public Guest2 GetGuestByUserId(int userId)
        {
            return guest2Repository.GetByUserId(userId);
        }
        private void InitializeUser()
        {
            foreach (var item in guest2Repository.GetAll())
            {
                item.User = userRepository.GetById(item.UserId);
            }
        }

        public List<Guest2> GetAllGuests()
        {
            return guest2Repository.GetAll();
        }
        public void Create(Guest2 guest)
        {
            guest2Repository.Create(guest);
        }
        public void Delete(Guest2 guest)
        {
            guest2Repository.Remove(guest);
        }
        public void Update(Guest2 guest)
        {
            guest2Repository.Update(guest);
        }
        public void Subscribe(IObserver observer)
        {
            guest2Repository.Subscribe(observer);
        }
        public Guest2 GetGuestById(int id)
        {
            return guest2Repository.GetById(id);
        }
        public int GetAgeOnTour(Guest2 guest,Tour tour)
        {
            int age = tour.StartOfTheTour.Year - guest.BirthDate.Year;
            if (guest.BirthDate.DayOfYear > DateTime.Now.DayOfYear)
                age--;
            return age;
        }
        public void GiveVoucher(int id,int years)
        {
            Guest2 guest = guest2Repository.GetById(id);
            Voucher voucher = new Voucher(voucherService.NextId(),DateTime.Now,DateTime.Now.AddYears(years),false,true);
            voucherService.Create(voucher);
            guest.VoucherIds.Add(voucher.Id);
            Update(guest);
        }
        public void GiveVoucherForGuestWhenFiveTimePresent(int id)
        {
            Guest2 guest = guest2Repository.GetById(id);
            Voucher voucher = new Voucher(voucherService.NextId(), DateTime.Now, DateTime.Now.AddMonths(6), false,true);
            voucherService.Create(voucher);
            guest.VoucherIds.Add(voucher.Id);
            Update(guest);
        }
    }
}

