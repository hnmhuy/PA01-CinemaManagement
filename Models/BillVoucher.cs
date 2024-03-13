using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.Models
{
    public partial class BillVoucher
    {
        public int BillId { get; set; }

        public int VoucherId { get; set; }

        public DateTime? AppliedTime { get; set; }

        public virtual Bill Bill { get; set; } = null!;

        public virtual Voucher Voucher { get; set; } = null!;
    }
}
