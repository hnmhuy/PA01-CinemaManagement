using System;
using System.Collections.Generic;

namespace CinemaManagement.Models;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string? Fullname { get; set; }

    public bool IsAdmin { get; set; }

    public int AccountId { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
