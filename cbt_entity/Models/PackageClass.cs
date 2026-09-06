using System;

namespace cbt.entity.Models;

/// <summary>
/// Relasi many-to-many paket ujian dan kelas.
/// </summary>
public partial class PackageClass
{
    public Guid Id { get; set; }

    public Guid ExamPackageId { get; set; }

    public string KodeClass { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ExamPackage ExamPackage { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;
}
