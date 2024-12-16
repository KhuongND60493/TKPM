using Microsoft.EntityFrameworkCore;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Exam> Exams { get; set; } = null!;
    public DbSet<SectionPart> SectionParts { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<QuestionOption> QuestionOptions { get; set; } = null!;
    public DbSet<StudentAttempt> StudentAttempts { get; set; } = null!;
    public DbSet<Answer> Answers { get; set; } = null!;
    public DbSet<WritingAssessment> WritingAssessments { get; set; } = null!;
    public DbSet<SpeakingAssessment> SpeakingAssessments { get; set; } = null!;
    public DbSet<StudentAttemptDetail> StudentAttemptDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // User configuration
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasConversion<int>();

            entity.HasMany(u => u.CreatedExams)
                .WithOne(e => e.CreatedBy)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(u => u.StudentAttempts)
                .WithOne(sa => sa.User)
                .HasForeignKey(sa => sa.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Exam configuration
        builder.Entity<Exam>(entity =>
        {
            entity.ToTable("Exams");

            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasMany(e => e.SectionParts)
                .WithOne(s => s.Exam)
                .HasForeignKey(s => s.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.StudentAttempts)
                .WithOne(sa => sa.Exam)
                .HasForeignKey(sa => sa.ExamId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SectionPart configuration
        builder.Entity<SectionPart>(entity =>
        {
            entity.ToTable("SectionParts");

            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Instructions);
            entity.Property(e => e.Content);
            entity.Property(e => e.SectionType)
                .HasConversion<int>();
            entity.Property(e => e.Type)
                .HasConversion<int>();

            entity.HasOne(s => s.Parent)
                .WithMany(s => s.Children)
                .HasForeignKey(s => s.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Question configuration
        builder.Entity<Question>(entity =>
        {
            entity.ToTable("Questions");

            entity.Property(e => e.QuestionText);
            entity.Property(e => e.Point);

            entity.HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(o => o.QuestionId);
        });

        // QuestionOption configuration
        builder.Entity<QuestionOption>(entity =>
        {
            entity.ToTable("QuestionOptions");

            entity.Property(e => e.Content);
            entity.Property(e => e.IsCorrect)
                .HasDefaultValue(false);
        });

        // StudentAttempt configuration
        builder.Entity<StudentAttempt>(entity =>
        {
            entity.ToTable("StudentAttempts");

            entity.Property(e => e.Status)
                .HasConversion<int>();

            entity.HasMany(sa => sa.Answers)
                .WithOne(a => a.Attempt)
                .HasForeignKey(a => a.AttemptId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Answer configuration
        builder.Entity<Answer>(entity =>
        {
            entity.ToTable("Answers");

            entity.Property(e => e.EssayAnswer);
            entity.Property(e => e.AiFeedback);
            entity.Property(e => e.Score);

            entity.HasOne(a => a.SelectedOption)
                .WithMany()
                .HasForeignKey(a => a.QuestionOptionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // WritingAssessment configuration
        builder.Entity<WritingAssessment>(entity =>
        {
            entity.ToTable("WritingAssessments");

            entity.Property(e => e.TaskAchievement)
                .HasColumnType("decimal(4,2)");
            entity.Property(e => e.CoherenceCohesion)
                .HasColumnType("decimal(4,2)");
            entity.Property(e => e.LexicalResource)
                .HasColumnType("decimal(4,2)");
            entity.Property(e => e.GrammarAccuracy)
                .HasColumnType("decimal(4,2)");
            entity.Property(e => e.DetailedFeedback);
        });

        builder.Entity<SpeakingAssessment>(entity =>
        {
            entity.ToTable("SpeakingAssessments");

            // Score columns with precision(5,2) for 0-100 range
            entity.Property(e => e.Pronunciation).HasColumnType("decimal(4,2)");
            entity.Property(e => e.Fluency).HasColumnType("decimal(4,2)");
            entity.Property(e => e.Accuracy).HasColumnType("decimal(4,2)");
            entity.Property(e => e.Prosody).HasColumnType("decimal(4,2)");
            entity.Property(e => e.Vocabulary).HasColumnType("decimal(4,2)");
            entity.Property(e => e.Grammar).HasColumnType("decimal(4,2)");
            entity.Property(e => e.TopicScore).HasColumnType("decimal(4,2)");

            // Text columns
            entity.Property(e => e.DetailedFeedback);
            entity.Property(e => e.TranscribedText);
            entity.Property(e => e.AudioUrl).HasMaxLength(255);

            // Word details as JSONB
            entity.Property(e => e.WordDetails).HasColumnType("jsonb");

            // Relationship
            entity.HasOne(w => w.Answer)
                .WithMany()
                .HasForeignKey(w => w.AnswerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}