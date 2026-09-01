using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

public partial class VAttemptDetail
{
    public Guid? AttemptId { get; set; }

    public short? OrderIndex { get; set; }

    public string? QuestionText { get; set; }

    public Guid? QuestionId { get; set; }

    public Guid? SelectedOptionId { get; set; }

    public string? StudentAnswerText { get; set; }

    public char? StudentOptionLabel { get; set; }

    public string? CorrectAnswerText { get; set; }

    public char? CorrectOptionLabel { get; set; }

    public bool? IsCorrect { get; set; }

    public short? Score { get; set; }
}
