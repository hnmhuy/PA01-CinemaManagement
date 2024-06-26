﻿using System;
using System.Collections.Generic;

namespace CinemaManagement.Models;

public partial class Bill
{
    public double Total { get; set; }

    public int AccountId { get; set; }

    public DateTime BookingTime { get; set; }

    public int BillId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<BillVoucher> BillVouchers { get; set; } = new List<BillVoucher>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
