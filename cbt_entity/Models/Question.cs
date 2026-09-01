using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Soal-soal dalam setiap paket ujian.
/// </summary>
public partial class Question
{
    public Guid Id { get; set; }

    public Guid ExamPackageId { get; set; }

    public string QuestionText { get; set; } = null!;

    public QuestionType Type { get; set; }

    /// <summary>
    /// Urutan tampil soal, unik per paket ujian.
    /// </summary>
    public short OrderIndex { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ExamPackage ExamPackage { get; set; } = null!;

    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
