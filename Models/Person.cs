using System;
using System.Collections.Generic;

namespace CinemaManagement.Models;

public partial class Person
{
    public string? Fullname { get; set; }

    public string? AvatarPath { get; set; }

    public string? Biography { get; set; }

    public int PersonId { get; set; }

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();

    public virtual ICollection<Movie> MoviesNavigation { get; set; } = new List<Movie>();
}
