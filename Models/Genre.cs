using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.Models
{
    public partial class Genre
    {
        public string? GenreName { get; set; }

        public int GenreId { get; set; }

        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();

        public virtual ICollection<Movie> MoviesNavigation { get; set; } = new List<Movie>();
    }
}
