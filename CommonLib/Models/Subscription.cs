using System;
using System.Collections.Generic;

namespace CommonLib.Models;

public partial class Subscription
{
    public Guid SubscriptionId { get; set; }

    public Guid BookId { get; set; }

    public Guid UserId { get; set; }

    public string SubscriptionDate { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
