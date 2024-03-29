﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSims.Domain.Model
{
    public class DateRanges
    {
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }

        public DateRanges() { }
        public DateRanges(DateOnly checkIn, DateOnly checkOut)
        {
            CheckIn = checkIn;
            CheckOut = checkOut;
        }
    }
}
