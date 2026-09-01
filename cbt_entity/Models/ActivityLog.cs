using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Feed aktivitas untuk dashboard admin. Di-insert oleh backend setelah event penting.
/// </summary>
public partial class ActivityLog
{
    public Guid Id { get; set; }

    public string Message { get; set; } = null!;

    public DateTime OccurredAt { get; set; }

    public Guid? RelatedUser { get; set; }

    public Guid? RelatedExam { get; set; }

    public virtual ExamPackage? RelatedExamNavigation { get; set; }

    public virtual User? RelatedUserNavigation { get; set; }
}
