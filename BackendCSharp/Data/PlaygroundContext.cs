using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace BackendCSharp.Data;

public partial class PlaygroundContext : DbContext
{
    public PlaygroundContext()
    {
    }

    public PlaygroundContext(DbContextOptions<PlaygroundContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Filter> Filters { get; set; }

    public virtual DbSet<FilterValue> FilterValues { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentFilter> StudentFilters { get; set; }

    public virtual DbSet<StudentGrade> StudentGrades { get; set; }

    public virtual DbSet<StudentSubject> StudentSubjects { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionStringBuilder = new DbConnectionStringBuilder
        {
            { "Host", Environment.GetEnvironmentVariable("POSTGRES_SERVER") ?? string.Empty },
            { "Database", Environment.GetEnvironmentVariable("PROJECT_DB") ?? string.Empty },
            { "Username", Environment.GetEnvironmentVariable("POSTGRES_USER") ?? string.Empty },
            { "Password", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? string.Empty },
            { "Port", Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? string.Empty }
        };

        optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Filter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("filters_pkey");

            entity.ToTable("filters");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<FilterValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("filter_values_pkey");

            entity.ToTable("filter_values");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.FilterId).HasColumnName("filter_id");
            entity.Property(e => e.Text)
                .HasMaxLength(255)
                .HasColumnName("text");

            entity.HasOne(d => d.Filter).WithMany(p => p.FilterValues)
                .HasForeignKey(d => d.FilterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("filter_values_filter_id_fkey");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("grades_pkey");

            entity.ToTable("grades");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Points)
                .HasPrecision(10, 5)
                .HasColumnName("points");
            entity.Property(e => e.Text)
                .HasMaxLength(255)
                .HasColumnName("text");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("students_pkey");

            entity.ToTable("students");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
        });

        modelBuilder.Entity<StudentFilter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("student_filters_pkey");

            entity.ToTable("student_filters");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.FilterId).HasColumnName("filter_id");
            entity.Property(e => e.FilterValueId).HasColumnName("filter_value_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Filter).WithMany(p => p.StudentFilters)
                .HasForeignKey(d => d.FilterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_filters_filter_id_fkey");

            entity.HasOne(d => d.FilterValue).WithMany(p => p.StudentFilters)
                .HasForeignKey(d => d.FilterValueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_filters_filter_value_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentFilters)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_filters_student_id_fkey");
        });

        modelBuilder.Entity<StudentGrade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("student_grades_pkey");

            entity.ToTable("student_grades");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.GradeId).HasColumnName("grade_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Grade).WithMany(p => p.StudentGrades)
                .HasForeignKey(d => d.GradeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_grades_grade_id_fkey");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentGrades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_grades_student_id_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.StudentGrades)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_grades_subject_id_fkey");
        });

        modelBuilder.Entity<StudentSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("student_subjects_pkey");

            entity.ToTable("student_subjects");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentSubjects)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_subjects_student_id_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.StudentSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_subjects_subject_id_fkey");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subjects_pkey");

            entity.ToTable("subjects");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
