using System;
using System.Collections.Generic;

namespace CinemaManagement.Models;

public partial class Genre
{
    public string GenreName { get; set; } = null!;

    public int GenreId { get; set; }

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
}
