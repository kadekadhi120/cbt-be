using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Log setiap kejadian pelanggaran anti-cheat.
/// </summary>
public partial class ProctoringLog
{
    public Guid Id { get; set; }

    public Guid AttemptId { get; set; }

    public ViolationType ViolationType { get; set; }

    public DateTime OccurredAt { get; set; }

    /// <summary>
    /// Urutan strike pada attempt ini saat pelanggaran terjadi.
    /// </summary>
    public short StrikeNumber { get; set; }

    public virtual ExamAttempt Attempt { get; set; } = null!;
}
