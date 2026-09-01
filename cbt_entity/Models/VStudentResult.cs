using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

public partial class VStudentResult
{
    public Guid? AttemptId { get; set; }

    public Guid? StudentId { get; set; }

    public string? StudentName { get; set; }

    public string? StudentClass { get; set; }

    public Guid? ExamPackageId { get; set; }

    public string? ExamTitle { get; set; }

    public DateTime? CompletedAt { get; set; }

    public short? Score { get; set; }

    public short? TotalScore { get; set; }

    public short? StrikeCount { get; set; }

    public string? ResultStatus { get; set; }
}
