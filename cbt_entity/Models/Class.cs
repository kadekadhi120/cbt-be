using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Master data kelas yang tersedia di platform.
/// </summary>
public partial class Class
{
    public string KodeClass { get; set; } = null!;

    public string ClassName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<PackageClass> PackageClasses { get; set; } = new List<PackageClass>();
}
