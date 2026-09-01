using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Paket ujian yang dibuat admin.
/// </summary>
public partial class ExamPackage
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public short DurationMinutes { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Cache jumlah soal, diperbarui otomatis via trigger.
    /// </summary>
    public short QuestionCount { get; set; }

    /// <summary>
    /// Cache jumlah attempt selesai, diperbarui otomatis via trigger.
    /// </summary>
    public int ParticipantCount { get; set; }

    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
