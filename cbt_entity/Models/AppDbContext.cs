using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace cbt.entity.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<AppSetting> AppSettings { get; set; }

    public virtual DbSet<ExamAttempt> ExamAttempts { get; set; }

    public virtual DbSet<ExamPackage> ExamPackages { get; set; }

    public virtual DbSet<ProctoringLog> ProctoringLogs { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<StudentAnswer> StudentAnswers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VAttemptDetail> VAttemptDetails { get; set; }

    public virtual DbSet<VDashboardStat> VDashboardStats { get; set; }

    public virtual DbSet<VStudentResult> VStudentResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=cbt;Username=cbt;Password=dalung04");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("activity_type", new[] { "info", "success", "warning", "danger" })
            .HasPostgresEnum("attempt_status", new[] { "in_progress", "submitted", "force_submitted", "time_expired" })
            .HasPostgresEnum("exam_status", new[] { "draft", "published", "closed" })
            .HasPostgresEnum("question_type", new[] { "multiple_choice" })
            .HasPostgresEnum("submit_type", new[] { "manual", "auto_time_expired", "force_anticheat" })
            .HasPostgresEnum("user_role", new[] { "admin", "student" })
            .HasPostgresEnum("user_status", new[] { "active", "inactive" })
            .HasPostgresEnum("violation_type", new[] { "tab_switch", "window_blur", "force_submit" })
            .HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("activity_logs_pkey");

            entity.ToTable("activity_logs", tb => tb.HasComment("Feed aktivitas untuk dashboard admin. Di-insert oleh backend setelah event penting."));

            entity.HasIndex(e => e.OccurredAt, "idx_activity_logs_occurred").IsDescending();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("occurred_at");
            entity.Property(e => e.RelatedExam).HasColumnName("related_exam");
            entity.Property(e => e.RelatedUser).HasColumnName("related_user");

            entity.HasOne(d => d.RelatedExamNavigation).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.RelatedExam)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("activity_logs_related_exam_fkey");

            entity.HasOne(d => d.RelatedUserNavigation).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.RelatedUser)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("activity_logs_related_user_fkey");
        });

        modelBuilder.Entity<AppSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("app_settings_pkey");

            entity.ToTable("app_settings", tb => tb.HasComment("Konfigurasi global aplikasi. Selalu satu baris dengan id = 1."));

            entity.Property(e => e.Id)
                .HasDefaultValue((short)1)
                .HasColumnName("id");
            entity.Property(e => e.MaintenanceMessage)
                .HasDefaultValueSql("'Platform sedang dalam pemeliharaan. Silakan kembali beberapa saat lagi.'::text")
                .HasColumnName("maintenance_message");
            entity.Property(e => e.MaintenanceMode)
                .HasDefaultValue(false)
                .HasComment("TRUE = halaman siswa menampilkan maintenance page.")
                .HasColumnName("maintenance_mode");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AppSettings)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("app_settings_updated_by_fkey");
        });

        modelBuilder.Entity<ExamAttempt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("exam_attempts_pkey");

            entity.ToTable("exam_attempts", tb => tb.HasComment("Sesi pengerjaan ujian oleh siswa."));

            entity.HasIndex(e => new { e.StudentId, e.ExamPackageId }, "exam_attempts_unique_per_student").IsUnique();

            entity.HasIndex(e => e.ExamPackageId, "idx_exam_attempts_exam");

            entity.HasIndex(e => e.StudentId, "idx_exam_attempts_student");

            entity.HasIndex(e => e.SubmittedAt, "idx_exam_attempts_submitted")
                .IsDescending()
                .HasFilter("(submitted_at IS NOT NULL)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ExamPackageId).HasColumnName("exam_package_id");
            entity.Property(e => e.Score)
                .HasComment("Skor 0â€“100. NULL selama in_progress.")
                .HasColumnName("score");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("started_at");
            entity.Property(e => e.StrikeCount)
                .HasDefaultValue((short)0)
                .HasComment("Akumulasi pelanggaran anti-cheat. Force submit jika > 3.")
                .HasColumnName("strike_count");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubmittedAt).HasColumnName("submitted_at");
            entity.Property(e => e.TotalScore)
                .HasDefaultValue((short)100)
                .HasColumnName("total_score");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ExamPackage).WithMany(p => p.ExamAttempts)
                .HasForeignKey(d => d.ExamPackageId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("exam_attempts_exam_package_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.ExamAttempts)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("exam_attempts_student_id_fkey");
        });

        modelBuilder.Entity<ExamPackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("exam_packages_pkey");

            entity.ToTable("exam_packages", tb => tb.HasComment("Paket ujian yang dibuat admin."));

            entity.HasIndex(e => e.CreatedBy, "idx_exam_packages_created_by");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("''::text")
                .HasColumnName("description");
            entity.Property(e => e.DurationMinutes).HasColumnName("duration_minutes");
            entity.Property(e => e.ParticipantCount)
                .HasDefaultValue(0)
                .HasComment("Cache jumlah attempt selesai, diperbarui otomatis via trigger.")
                .HasColumnName("participant_count");
            entity.Property(e => e.QuestionCount)
                .HasDefaultValue((short)0)
                .HasComment("Cache jumlah soal, diperbarui otomatis via trigger.")
                .HasColumnName("question_count");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ExamPackages)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("exam_packages_created_by_fkey");
        });

        modelBuilder.Entity<ProctoringLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proctoring_logs_pkey");

            entity.ToTable("proctoring_logs", tb => tb.HasComment("Log setiap kejadian pelanggaran anti-cheat."));

            entity.HasIndex(e => new { e.AttemptId, e.OccurredAt }, "idx_proctoring_logs_attempt");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AttemptId).HasColumnName("attempt_id");
            entity.Property(e => e.OccurredAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("occurred_at");
            entity.Property(e => e.StrikeNumber)
                .HasComment("Urutan strike pada attempt ini saat pelanggaran terjadi.")
                .HasColumnName("strike_number");

            entity.HasOne(d => d.Attempt).WithMany(p => p.ProctoringLogs)
                .HasForeignKey(d => d.AttemptId)
                .HasConstraintName("proctoring_logs_attempt_id_fkey");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questions_pkey");

            entity.ToTable("questions", tb => tb.HasComment("Soal-soal dalam setiap paket ujian."));

            entity.HasIndex(e => new { e.ExamPackageId, e.OrderIndex }, "idx_questions_exam_package");

            entity.HasIndex(e => new { e.ExamPackageId, e.OrderIndex }, "questions_unique_order").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ExamPackageId).HasColumnName("exam_package_id");
            entity.Property(e => e.OrderIndex)
                .HasComment("Urutan tampil soal, unik per paket ujian.")
                .HasColumnName("order_index");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ExamPackage).WithMany(p => p.Questions)
                .HasForeignKey(d => d.ExamPackageId)
                .HasConstraintName("questions_exam_package_id_fkey");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("question_options_pkey");

            entity.ToTable("question_options", tb => tb.HasComment("Pilihan jawaban untuk setiap soal."));

            entity.HasIndex(e => e.QuestionId, "idx_question_options_one_correct")
                .IsUnique()
                .HasFilter("(is_correct = true)");

            entity.HasIndex(e => e.QuestionId, "idx_question_options_question");

            entity.HasIndex(e => new { e.QuestionId, e.Label }, "question_options_unique_label").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsCorrect)
                .HasDefaultValue(false)
                .HasComment("Hanya satu opsi per soal yang bernilai TRUE (dijamin via partial unique index).")
                .HasColumnName("is_correct");
            entity.Property(e => e.Label)
                .HasMaxLength(1)
                .HasComment("Label opsi: A, B, C, D, atau E.")
                .HasColumnName("label");
            entity.Property(e => e.OptionText).HasColumnName("option_text");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithOne(p => p.QuestionOption)
                .HasForeignKey<QuestionOption>(d => d.QuestionId)
                .HasConstraintName("question_options_question_id_fkey");
        });

        modelBuilder.Entity<StudentAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("student_answers_pkey");

            entity.ToTable("student_answers", tb => tb.HasComment("Jawaban yang dipilih siswa per soal."));

            entity.HasIndex(e => e.AttemptId, "idx_student_answers_attempt");

            entity.HasIndex(e => e.QuestionId, "idx_student_answers_question");

            entity.HasIndex(e => new { e.AttemptId, e.QuestionId }, "student_answers_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AnsweredAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("answered_at");
            entity.Property(e => e.AttemptId).HasColumnName("attempt_id");
            entity.Property(e => e.IsCorrect)
                .HasComment("Dikalkulasi saat submit: bandingkan selected_option_id dengan is_correct di question_options.")
                .HasColumnName("is_correct");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.Score)
                .HasDefaultValue((short)0)
                .HasColumnName("score");
            entity.Property(e => e.SelectedOptionId)
                .HasComment("NULL jika soal tidak dijawab.")
                .HasColumnName("selected_option_id");

            entity.HasOne(d => d.Attempt).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.AttemptId)
                .HasConstraintName("student_answers_attempt_id_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("student_answers_question_id_fkey");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.StudentAnswers)
                .HasForeignKey(d => d.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("student_answers_selected_option_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", tb => tb.HasComment("Semua pengguna platform: admin dan siswa."));

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.Email, "users_email_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.Class)
                .HasMaxLength(50)
                .HasComment("Kelas/kelompok siswa, contoh: XII IPA 1. NULL untuk admin.")
                .HasColumnName("class");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasComment("Hash bcrypt dari password. JANGAN simpan plain text.")
                .HasColumnName("password_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<VAttemptDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_attempt_detail");

            entity.Property(e => e.AttemptId).HasColumnName("attempt_id");
            entity.Property(e => e.CorrectAnswerText).HasColumnName("correct_answer_text");
            entity.Property(e => e.CorrectOptionLabel)
                .HasMaxLength(1)
                .HasColumnName("correct_option_label");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.SelectedOptionId).HasColumnName("selected_option_id");
            entity.Property(e => e.StudentAnswerText).HasColumnName("student_answer_text");
            entity.Property(e => e.StudentOptionLabel)
                .HasMaxLength(1)
                .HasColumnName("student_option_label");
        });

        modelBuilder.Entity<VDashboardStat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_dashboard_stats");

            entity.Property(e => e.ActiveExams).HasColumnName("active_exams");
            entity.Property(e => e.TodayAttempts).HasColumnName("today_attempts");
            entity.Property(e => e.TotalCompletedAttempts).HasColumnName("total_completed_attempts");
            entity.Property(e => e.TotalExamPackages).HasColumnName("total_exam_packages");
            entity.Property(e => e.TotalStudents).HasColumnName("total_students");
        });

        modelBuilder.Entity<VStudentResult>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_student_results");

            entity.Property(e => e.AttemptId).HasColumnName("attempt_id");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.ExamPackageId).HasColumnName("exam_package_id");
            entity.Property(e => e.ExamTitle)
                .HasMaxLength(255)
                .HasColumnName("exam_title");
            entity.Property(e => e.ResultStatus).HasColumnName("result_status");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.StrikeCount).HasColumnName("strike_count");
            entity.Property(e => e.StudentClass)
                .HasMaxLength(50)
                .HasColumnName("student_class");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.StudentName)
                .HasMaxLength(150)
                .HasColumnName("student_name");
            entity.Property(e => e.TotalScore).HasColumnName("total_score");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
