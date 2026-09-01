using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Sesi pengerjaan ujian oleh siswa.
/// </summary>
public partial class ExamAttempt
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid ExamPackageId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    /// <summary>
    /// Skor 0â€“100. NULL selama in_progress.
    /// </summary>
    public short? Score { get; set; }

    public short TotalScore { get; set; }

    /// <summary>
    /// Akumulasi pelanggaran anti-cheat. Force submit jika &gt; 3.
    /// </summary>
    public short StrikeCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ExamPackage ExamPackage { get; set; } = null!;

    public virtual ICollection<ProctoringLog> ProctoringLogs { get; set; } = new List<ProctoringLog>();

    public virtual User Student { get; set; } = null!;

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
