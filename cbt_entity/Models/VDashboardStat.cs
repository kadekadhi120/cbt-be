using System;
using System.Collections.Generic;

namespace cbt.entity.Models;

public partial class VDashboardStat
{
    public long? TotalStudents { get; set; }

    public long? TotalExamPackages { get; set; }

    public long? ActiveExams { get; set; }

    public long? TodayAttempts { get; set; }

    public long? TotalCompletedAttempts { get; set; }
}
