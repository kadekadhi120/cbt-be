namespace cbt.entity.Models;

public enum UserRole
{
    admin,
    student
}

public enum UserStatus
{
    active,
    inactive
}

public enum ExamStatus
{
    draft,
    published,
    closed
}

public enum QuestionType
{
    multiple_choice
}

public enum AttemptStatus
{
    in_progress,
    submitted,
    force_submitted,
    time_expired
}

public enum SubmitType
{
    manual,
    auto_time_expired,
    force_anticheat
}

public enum ViolationType
{
    tab_switch,
    window_blur,
    force_submit
}

public enum ActivityType
{
    info,
    success,
    warning,
    danger
}
