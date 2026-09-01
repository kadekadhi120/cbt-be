using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Konfigurasi global aplikasi. Selalu satu baris dengan id = 1.
/// </summary>
public partial class AppSetting
{
    public short Id { get; set; }

    /// <summary>
    /// TRUE = halaman siswa menampilkan maintenance page.
    /// </summary>
    public bool MaintenanceMode { get; set; }

    public string MaintenanceMessage { get; set; } = null!;

    public Guid? UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
