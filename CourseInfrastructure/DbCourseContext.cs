using System;
using System.Collections.Generic;
using CourseDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseInfrastructure;

public partial class DbCourseContext : DbContext
{
    public DbCourseContext()
    {
    }

    public DbCourseContext(DbContextOptions<DbCourseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountScourse> AccountScourses { get; set; }

    public virtual DbSet<Certificate> Certificates { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseAccount> CourseAccounts { get; set; }

    public virtual DbSet<Excercise> Excercises { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=labdb;Username=postgres;Password=1234;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("account_role", new[] { "Admin", "Teacher", "Student" });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Accounts_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<AccountScourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Account_Scourses_pkey");

            entity.ToTable("Account_Scourses");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Account).WithMany(p => p.AccountScourses)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("AccountFk");

            entity.HasOne(d => d.Score).WithMany(p => p.AccountScourses)
                .HasForeignKey(d => d.ScoreId)
                .HasConstraintName("ScoreFk");
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Certificates_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Account).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AccountFk");

            entity.HasOne(d => d.Course).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CourseFk");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Courses_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Author).WithMany(p => p.Courses)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("AuthorFk");
        });

        modelBuilder.Entity<CourseAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Course_Accounts_pkey");

            entity.ToTable("Course_Accounts");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Account).WithMany(p => p.CourseAccounts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AccountFk");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseAccounts)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CourseFk");
        });

        modelBuilder.Entity<Excercise>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Excercises_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Course).WithMany(p => p.Excercises)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("CourseFk");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Scores_pkey");

            entity.HasIndex(e => e.ExcersiceId, "ExcersiceUnique").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Excersice).WithOne(p => p.Score)
                .HasForeignKey<Score>(d => d.ExcersiceId)
                .HasConstraintName("ExcersiseFk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
