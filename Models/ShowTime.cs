using System;
using System.Collections.Generic;

namespace CinemaManagement.Models;

public partial class ShowTime
{
    public int ShowTimeId { get; set; }

    public int MovieId { get; set; }

    public DateTime ShowDate { get; set; }

    public int MaxRow { get; set; }

    public int MaxCol { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
