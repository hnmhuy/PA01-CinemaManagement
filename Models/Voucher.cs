using System;
using System.Collections.Generic;

namespace CinemaManagement.Models;

public partial class Voucher
{
    public string? VoucherCode { get; set; }

    public double? DiscountAmount { get; set; }

    public bool? IsExpired { get; set; }

    public bool? IsPercentageDiscount { get; set; }

    public double? RequirementAmount { get; set; }

    public int? VoucherAmount { get; set; }

    public int VoucherId { get; set; }

    public virtual ICollection<BillVoucher> BillVouchers { get; set; } = new List<BillVoucher>();
}
