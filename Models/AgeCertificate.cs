﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.Models
{
    public partial class AgeCertificate
    {
        public string? DisplayContent { get; set; }

        public int? RequireAge { get; set; }

        public int AgeCertificateId { get; set; }

        public string? BackgroundColor { get; set; }

        public string? ForegroundColor { get; set; }

        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
