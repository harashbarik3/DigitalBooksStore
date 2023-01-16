using System;
using System.Collections.Generic;

namespace CommonLib.Models;

public partial class Publisher
{
    public Guid PublisherId { get; set; }

    public string PublisherName { get; set; } = null!;

    public virtual ICollection<Book> Books { get; } = new List<Book>();
}
