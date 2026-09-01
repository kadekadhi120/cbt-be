using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

/// <summary>
/// Jawaban yang dipilih siswa per soal.
/// </summary>
public partial class StudentAnswer
{
    public Guid Id { get; set; }

    public Guid AttemptId { get; set; }

    public Guid QuestionId { get; set; }

    /// <summary>
    /// NULL jika soal tidak dijawab.
    /// </summary>
    public Guid? SelectedOptionId { get; set; }

    /// <summary>
    /// Dikalkulasi saat submit: bandingkan selected_option_id dengan is_correct di question_options.
    /// </summary>
    public bool? IsCorrect { get; set; }

    public short Score { get; set; }

    public DateTime AnsweredAt { get; set; }

    public virtual ExamAttempt Attempt { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    public virtual QuestionOption? SelectedOption { get; set; }
}
