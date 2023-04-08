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
    public class Guest2Service
    {
        private Guest2Repository guests;
        private VoucherService voucherService;

        public Guest2Service()
        {
            guests = new Guest2Repository();
            voucherService = new VoucherService();
        }

        public List<Guest2> GetAllGuests()
        {
            return guests.GetAll();
        }

        public void Create(Guest2 guest)
        {
            guests.Add(guest);
        }

        public void Delete(Guest2 guest)
        {
            guests.Remove(guest);
        }

        public void Update(Guest2 guest)
        {
            guests.Update(guest);
        }

        public void Subscribe(IObserver observer)
        {
            guests.Subscribe(observer);
        }

        public Guest2 FindGuest2ById(int id)
        {
            return guests.FindById(id);
        }
        public void GiveVoucher(int id)
        {
            Guest2 guest = guests.FindById(id);
            Voucher voucher = new Voucher(-1, DateTime.Now.AddYears(1));
            voucherService.Create(voucher);
            guest.VoucherIds.Add(voucher.Id);
            Update(guest);
        }

    }
}
