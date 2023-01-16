using System;
using System.Collections.Generic;

namespace CommonLib.Models;

public partial class Book
{
    public Guid BookId { get; set; }

    public string BookName { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public decimal Price { get; set; }

    public Guid PublisherId { get; set; }

    public string PublishedDate { get; set; } = null!;

    public string BookContent { get; set; } = null!;

    public bool Active { get; set; }

    public Guid UserId { get; set; }

    public bool? IsBlocked { get; set; }

    public byte[]? Image { get; set; }

    public string? CategoryName { get; set; }

    public string? PublisherName { get; set; }

    public string? Auther { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<Subscription> Subscriptions { get; } = new List<Subscription>();

    public virtual User User { get; set; } = null!;
}
