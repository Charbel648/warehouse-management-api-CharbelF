using System;
using System.Collections.Generic;

namespace Warehouse.Infrastructure.Persistence.DbFirst.Models;

public partial class Supplier
{
    public Guid SupplierId { get; set; }

    public string Name { get; set; } = null!;

    public string? Country { get; set; }

    public string? ContactEmail { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
