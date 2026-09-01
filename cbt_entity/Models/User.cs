using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Semua pengguna platform: admin dan siswa.
/// </summary>
public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    /// <summary>
    /// Hash bcrypt dari password. JANGAN simpan plain text.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Kelas/kelompok siswa, contoh: XII IPA 1. NULL untuk admin.
    /// </summary>
    public string? Class { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public virtual ICollection<AppSetting> AppSettings { get; set; } = new List<AppSetting>();

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();

    public virtual ICollection<ExamPackage> ExamPackages { get; set; } = new List<ExamPackage>();
}
