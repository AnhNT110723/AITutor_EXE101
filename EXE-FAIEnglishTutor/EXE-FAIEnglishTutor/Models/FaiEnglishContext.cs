﻿using System;
using System.Collections.Generic;
using EXE_FAIEnglishTutor.Enums;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Models;

public partial class FaiEnglishContext : DbContext
{
    public FaiEnglishContext()
    {
    }

    public FaiEnglishContext(DbContextOptions<FaiEnglishContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamPart> ExamParts { get; set; }

    public virtual DbSet<ExamSection> ExamSections { get; set; }

    public virtual DbSet<ExamType> ExamTypes { get; set; }

    public virtual DbSet<Feeback> Feebacks { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Podcast> Podcasts { get; set; }

    public virtual DbSet<Progress> Progresses { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Situation> Situations { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAnswer> UserAnswers { get; set; }

    public virtual DbSet<UserExamPartSelection> UserExamPartSelections { get; set; }

    public virtual DbSet<UserExamResult> UserExamResults { get; set; }

    public virtual DbSet<VerificationToken> VerificationTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:FAI_ENGLISH");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__Answer__D48250247E727791");

            entity.ToTable("Answer");

            entity.Property(e => e.AnswerId).HasColumnName("AnswerID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__Answer__Question__00200768");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D7187D5B399B4");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseName).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.Type).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK__Courses__TypeID__4E88ABD4");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exam__297521A767E08970");

            entity.ToTable("Exam");

            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExamTypeId).HasColumnName("ExamTypeID");
            entity.Property(e => e.ParentExamId).HasColumnName("ParentExamID");
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.ExamType).WithMany(p => p.Exams)
                .HasForeignKey(d => d.ExamTypeId)
                .HasConstraintName("FK__Exam__ExamTypeID__73BA3083");

            entity.HasOne(d => d.ParentExam).WithMany(p => p.InverseParentExam)
                .HasForeignKey(d => d.ParentExamId)
                .HasConstraintName("FK_Exam_Parent");
        });

        modelBuilder.Entity<ExamPart>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("PK__ExamPart__7C3F0D30096714BC");

            entity.ToTable("ExamPart");

            entity.Property(e => e.PartId).HasColumnName("PartID");
            entity.Property(e => e.ExamTypeId).HasColumnName("ExamTypeID");
            entity.Property(e => e.PartName).HasMaxLength(100);
            entity.Property(e => e.PartType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ExamType).WithMany(p => p.ExamParts)
                .HasForeignKey(d => d.ExamTypeId)
                .HasConstraintName("FK__ExamPart__ExamTy__70DDC3D8");
        });

        modelBuilder.Entity<ExamSection>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__ExamSect__80EF0892B15A706F");

            entity.ToTable("ExamSection");

            entity.Property(e => e.SectionId).HasColumnName("SectionID");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.PartId).HasColumnName("PartID");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamSections)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("FK__ExamSecti__ExamI__787EE5A0");

            entity.HasOne(d => d.Part).WithMany(p => p.ExamSections)
                .HasForeignKey(d => d.PartId)
                .HasConstraintName("FK__ExamSecti__PartI__797309D9");
        });

        modelBuilder.Entity<ExamType>(entity =>
        {
            entity.HasKey(e => e.ExamTypeId).HasName("PK__ExamType__087D51101EE6952B");

            entity.ToTable("ExamType");

            entity.Property(e => e.ExamTypeId).HasColumnName("ExamTypeID");
            entity.Property(e => e.ExamName).HasMaxLength(100);
        });

        modelBuilder.Entity<Feeback>(entity =>
        {
            entity.HasKey(e => e.FeebackId).HasName("PK__Feeback__FE6C68823FA6E61F");

            entity.ToTable("Feeback");

            entity.Property(e => e.FeebackId).HasColumnName("FeebackID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.Feebacks)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Feeback__CourseI__6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.Feebacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Feeback__UserID__693CA210");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("PK__Lessons__B084ACB0BC8CB684");

            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(500)
                .HasColumnName("VideoURL");

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Lessons__CourseI__5812160E");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Level__09F03C06B210D704");

            entity.ToTable("Level");

            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.LevelName).HasMaxLength(200);
            entity.Property(e => e.LevelScore).HasMaxLength(50);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A580A0F7EFD");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Content).HasMaxLength(200);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.Payments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Payments__Course__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Payments__UserID__5BE2A6F2");
        });

        modelBuilder.Entity<Podcast>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Podcast__3214EC0765B0BA87");

            entity.ToTable("Podcast");

            entity.Property(e => e.AudioUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Topic).HasMaxLength(100);
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("PK__Progress__BAE29C851E1FF083");

            entity.ToTable("Progress");

            entity.Property(e => e.ProgressId).HasColumnName("ProgressID");
            entity.Property(e => e.Completed).HasDefaultValue(false);
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Progress__Lesson__6477ECF3");

            entity.HasOne(d => d.User).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Progress__UserID__6383C8BA");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06F8C547F3BF9");

            entity.ToTable("Question");

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.AudioUrl)
                .HasMaxLength(500)
                .HasColumnName("AudioURL");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("ImageURL");
            entity.Property(e => e.QuestionType).HasMaxLength(50);
            entity.Property(e => e.SectionId).HasColumnName("SectionID");

            entity.HasOne(d => d.Section).WithMany(p => p.Questions)
                .HasForeignKey(d => d.SectionId)
                .HasConstraintName("FK__Question__Sectio__7C4F7684");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK__RefreshT__F5845E3975CB6193");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeviceInfo).HasMaxLength(256);
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRefreshToken");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A22DC8CBC");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(255);
        });

        modelBuilder.Entity<Situation>(entity =>
        {
            entity.HasKey(e => e.SituatuonId).HasName("PK__Situatio__7F5DBBC30F9F4DD5");

            entity.ToTable("Situation");

            entity.Property(e => e.SituatuonId).HasColumnName("SituatuonID");
            entity.Property(e => e.AudioUrl)
                .HasMaxLength(500)
                .HasColumnName("AudioURL");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.RoleAi)
                .HasMaxLength(255)
                .HasColumnName("RoleAI");
            entity.Property(e => e.RoleUser).HasMaxLength(255);
            entity.Property(e => e.SituationName).HasMaxLength(255);
            entity.Property(e => e.TypeId).HasColumnName("TypeID");

            entity.HasOne(d => d.Level).WithMany(p => p.Situations)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK__Situation__Level__5535A963");

            entity.HasOne(d => d.Type).WithMany(p => p.Situations)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK__Situation__TypeI__5441852A");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Type__516F0395AC29C4DA");

            entity.ToTable("Type");

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.TypeName).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC8A912926");

            entity.HasIndex(e => new { e.Provider, e.ProviderId }, "UQ_Provider_ProviderId")
                .IsUnique()
                .HasFilter("([ProviderId] IS NOT NULL)");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053422FCD401").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("expiryDate");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.ProviderId).HasMaxLength(255);
            entity.Property(e => e.Status).HasConversion(
            v => v.ToString(), // Enum -> string khi lưu DB
            v => (AccountStatus)Enum.Parse(typeof(AccountStatus), v) // string -> Enum khi lấy từ DB
        ); ;

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRole__roleId__403A8C7D"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRole__userId__3F466844"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__7743989DF52AE2E1");
                        j.ToTable("UserRole");
                        j.IndexerProperty<int>("UserId").HasColumnName("userId");
                        j.IndexerProperty<int>("RoleId").HasColumnName("roleId");
                    });
        });

        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.HasKey(e => e.UserAnswerId).HasName("PK__UserAnsw__47CE235F8581E26C");

            entity.ToTable("UserAnswer");

            entity.Property(e => e.UserAnswerId).HasColumnName("UserAnswerID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.SelectedAnswerId).HasColumnName("SelectedAnswerID");

            entity.HasOne(d => d.Question).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK__UserAnswe__Quest__0C85DE4D");

            entity.HasOne(d => d.Result).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.ResultId)
                .HasConstraintName("FK__UserAnswe__Resul__0B91BA14");

            entity.HasOne(d => d.SelectedAnswer).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.SelectedAnswerId)
                .HasConstraintName("FK__UserAnswe__Selec__0D7A0286");
        });

        modelBuilder.Entity<UserExamPartSelection>(entity =>
        {
            entity.HasKey(e => e.SelectionId).HasName("PK__UserExam__7F17912F84BAF3EC");

            entity.ToTable("UserExamPartSelection");

            entity.HasIndex(e => new { e.ResultId, e.PartId }, "UK_UserExamPartSelection").IsUnique();

            entity.Property(e => e.SelectionId).HasColumnName("SelectionID");
            entity.Property(e => e.PartId).HasColumnName("PartID");
            entity.Property(e => e.ResultId).HasColumnName("ResultID");

            entity.HasOne(d => d.Part).WithMany(p => p.UserExamPartSelections)
                .HasForeignKey(d => d.PartId)
                .HasConstraintName("FK__UserExamP__PartI__08B54D69");

            entity.HasOne(d => d.Result).WithMany(p => p.UserExamPartSelections)
                .HasForeignKey(d => d.ResultId)
                .HasConstraintName("FK__UserExamP__Resul__07C12930");
        });

        modelBuilder.Entity<UserExamResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__UserExam__97690228FF253609");

            entity.ToTable("UserExamResult");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Exam).WithMany(p => p.UserExamResults)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("FK__UserExamR__ExamI__03F0984C");

            entity.HasOne(d => d.User).WithMany(p => p.UserExamResults)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserExamR__UserI__02FC7413");
        });

        modelBuilder.Entity<VerificationToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__Verifica__AC16DB47A502C8F4");

            entity.ToTable("VerificationToken");

            entity.HasIndex(e => e.UserId, "UQ__Verifica__CB9A1CFE08CB1322").IsUnique();

            entity.Property(e => e.TokenId).HasColumnName("tokenId");
            entity.Property(e => e.ExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("expiryDate");
            entity.Property(e => e.TokenCode).HasColumnName("tokenCode");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithOne(p => p.VerificationToken)
                .HasForeignKey<VerificationToken>(d => d.UserId)
                .HasConstraintName("FK_UserToken");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
