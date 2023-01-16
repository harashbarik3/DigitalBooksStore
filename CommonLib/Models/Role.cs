using System;
using System.Collections.Generic;

namespace CommonLib.Models;

public partial class Role
{
    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; } = new List<User>();
}
