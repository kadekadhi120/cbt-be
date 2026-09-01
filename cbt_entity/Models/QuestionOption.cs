using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Pilihan jawaban untuk setiap soal.
/// </summary>
public partial class QuestionOption
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string OptionText { get; set; } = null!;

    /// <summary>
    /// Hanya satu opsi per soal yang bernilai TRUE (dijamin via partial unique index).
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Label opsi: A, B, C, D, atau E.
    /// </summary>
    public char Label { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
