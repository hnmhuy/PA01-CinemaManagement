using System;
using System.Collections.Generic;

namespace CinemaManagement.Models;

public partial class Movie
{
    public string? Title { get; set; }

    public int? Duration { get; set; }

    public int? PublishYear { get; set; }

    public double? Imdbrating { get; set; }

    public int? AgeCertificateId { get; set; }

    public int? DirectorId { get; set; }

    public int MovieId { get; set; }

    public bool? IsGoldenHour { get; set; }

    public bool? IsBlockbuster { get; set; }

    public string? PosterPath { get; set; }

    public string? TrailerPath { get; set; }

    public string? Description { get; set; }

    public virtual AgeCertificate? AgeCertificate { get; set; }

    public virtual Person? Director { get; set; }

    public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
