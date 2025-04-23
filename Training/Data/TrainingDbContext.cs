using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace Training.Data;

public partial class TrainingDbContext : DbContext
{
    public TrainingDbContext()
    {
    }

    public TrainingDbContext(DbContextOptions<TrainingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseRegistration> CourseRegistrations { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    //public DbSet<CourseModel> Course { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Accounts__349DA58681D77914");

            entity.HasIndex(e => e.Username, "IX_Accounts_Username");

            entity.HasIndex(e => e.Username, "UQ__Accounts__536C85E4D9FECC2B").IsUnique();

            entity.Property(e => e.AccountId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("AccountID");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71872BAE26F4");

            entity.HasIndex(e => e.CourseName, "IX_Courses_CourseName");

            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("CourseID");
            entity.Property(e => e.CourseImagePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CourseName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(true);
            entity.Property(e => e.TuitionFee).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<CourseRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__CourseRe__6EF588301A851FF2");

            entity.HasIndex(e => new { e.StudentId, e.CourseId }, "UQ_StudentCourse").IsUnique();

            entity.Property(e => e.RegistrationId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("RegistrationID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("CourseID");
            entity.Property(e => e.Status_Register)
                .HasMaxLength(50)
                .IsUnicode(true);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.StudentId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("StudentID");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseReg__Cours__4AB81AF0");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseRegistrations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CourseReg__Stude__49C3F6B7");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A7985F9E813");

            entity.HasIndex(e => e.Email, "IX_Students_Email");

            entity.HasIndex(e => e.Email, "UQ__Students__A9D10534A2223A05").IsUnique();

            entity.Property(e => e.StudentId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasColumnName("StudentID");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(true);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(true);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProfileImagePath)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.StudentNavigation).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Students__Studen__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

//public DbSet<CourseModel> CourseModel { get; set; } = default!;
}
