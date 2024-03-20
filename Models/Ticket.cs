using System;
using System.Collections.Generic;

namespace CinemaManagement.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public bool IsAvailable { get; set; }

    public string Row { get; set; } = null!;

    public int Col { get; set; }

    public double Price { get; set; }

    public int BillId { get; set; }

    public int ShowTimeId { get; set; }

    public bool IsVip { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual ShowTime ShowTime { get; set; } = null!;
}
