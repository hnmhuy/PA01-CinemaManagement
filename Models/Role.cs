using System;
using System.Collections.Generic;

namespace CinemaManagement.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Contributor> Contributors { get; set; } = new List<Contributor>();
}
