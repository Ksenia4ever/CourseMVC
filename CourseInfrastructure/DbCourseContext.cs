//using System;
//using System.Collections.Generic;
//using CourseDomain.Model;
//using Microsoft.EntityFrameworkCore;

//namespace CourseInfrastructure;

//public partial class DbCourseContext : DbContext
//{
//    public DbCourseContext()
//    {
//    }

//    public DbCourseContext(DbContextOptions<DbCourseContext> options)
//        : base(options)
//    {
//    }

//    public virtual DbSet<Account> Accounts { get; set; }

//    public virtual DbSet<AccountScourse> AccountScourses { get; set; }

//    public virtual DbSet<Certificate> Certificates { get; set; }

//    public virtual DbSet<Course> Courses { get; set; }

//    public virtual DbSet<CourseAccount> CourseAccounts { get; set; }

//    public virtual DbSet<Excercise> Excercises { get; set; }

//    public virtual DbSet<Score> Scores { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=labdb;Username=postgres;Password=1234;");

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<Account>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("Accounts_pkey");

//            entity.Property(e => e.Id).UseIdentityColumn();

//            entity.Property(e => e.Email)
//                .HasColumnName("Email");

//            entity.HasIndex(e => e.Email)
//                .IsUnique()
//                .HasDatabaseName("UQ_Accounts_Email");
//        });

//        modelBuilder.Entity<AccountScourse>(entity =>
//        {
//            entity.HasKey(e => new { e.AccountId, e.ScoreId }).HasName("Account_Scourses_pkey");

//            entity.ToTable("Account_Scourses");

//            entity.Property(e => e.AccountId).HasColumnName("AccountId");
//            entity.Property(e => e.ScoreId).HasColumnName("ScoreId");

//            entity.HasOne(d => d.Account).WithMany(p => p.AccountScourses)
//                .HasForeignKey(d => d.AccountId)
//                .OnDelete(DeleteBehavior.ClientSetNull)
//                .HasConstraintName("AccountFk");

//            entity.HasOne(d => d.Score).WithMany(p => p.AccountScourses)
//                .HasForeignKey(d => d.ScoreId)
//                .OnDelete(DeleteBehavior.ClientSetNull)
//                .HasConstraintName("ScoreFk");
//        });

//        modelBuilder.Entity<Certificate>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("Certificates_pkey");

//            // entity.Property(e => e.Id).ValueGeneratedOnAdd(); // вместо ValueGeneratedNever()
//            entity.Property(e => e.Id).UseIdentityColumn(); // или ValueGeneratedOnAdd()

//            entity.HasIndex(e => new { e.AccountId, e.CourseId })
//                  .IsUnique()
//                  .HasDatabaseName("UQ_Certificates_Account_Course");

//            entity.HasOne(d => d.Account).WithMany(p => p.Certificates)
//                .HasForeignKey(d => d.AccountId)
//                .OnDelete(DeleteBehavior.Cascade)
//                .HasConstraintName("AccountFk");

//            entity.HasOne(d => d.Course).WithMany(p => p.Certificates)
//                .HasForeignKey(d => d.CourseId)
//                .OnDelete(DeleteBehavior.Restrict)
//                .HasConstraintName("CourseFk");
//            entity.Property(e => e.IssuedDate)
//                .HasColumnName("IssuedDate")
//                .HasColumnType("timestamp without time zone");
//        });

//        modelBuilder.Entity<Course>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("Courses_pkey");

//            entity.Property(e => e.Id).ValueGeneratedNever();

//            entity.HasOne(d => d.Author).WithMany(p => p.Courses)
//                .HasForeignKey(d => d.AuthorId)
//                .HasConstraintName("AuthorFk");
//        });

//        modelBuilder.Entity<CourseAccount>(entity =>
//        {
//            entity.HasKey(e => new { e.CourseId, e.AccountId }).HasName("Course_Accounts_pkey");

//            entity.ToTable("Course_Accounts");

//            entity.Property(e => e.CourseId).HasColumnName("CourseId");
//            entity.Property(e => e.AccountId).HasColumnName("AccountId");

//            entity.Property(e => e.SubscribedAt)
//                .HasColumnName("SubscribedAt")
//                .HasColumnType("timestamp without time zone");

//            entity.HasOne(d => d.Account).WithMany(p => p.CourseAccounts)
//                .HasForeignKey(d => d.AccountId)
//                .OnDelete(DeleteBehavior.ClientSetNull)
//                .HasConstraintName("Course_Accounts_AccountId_fkey");

//            entity.HasOne(d => d.Course).WithMany(p => p.CourseAccounts)
//                .HasForeignKey(d => d.CourseId)
//                .OnDelete(DeleteBehavior.ClientSetNull)
//                .HasConstraintName("Course_Accounts_CourseId_fkey");
//        });
//        modelBuilder.Entity<Excercise>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("Excercises_pkey");

//            entity.Property(e => e.Id).ValueGeneratedNever();

//            entity.HasOne(d => d.Course).WithMany(p => p.Excercises)
//                .HasForeignKey(d => d.CourseId)
//                .HasConstraintName("CourseFk");
//        });

//        modelBuilder.Entity<Score>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("Scores_pkey");

//            entity.HasIndex(e => e.ExcersiceId, "ExcersiceUnique").IsUnique();

//            entity.Property(e => e.Id).ValueGeneratedNever();

//            entity.HasOne(d => d.Excersice).WithOne(p => p.Score)
//                .HasForeignKey<Score>(d => d.ExcersiceId)
//                .HasConstraintName("ExcersiseFk");
//        });

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}

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

    public virtual DbSet<TeacherRequest> TeacherRequests { get; set; }

    public virtual DbSet<TeacherRequestCourse> TeacherRequestCourses { get; set; }

    public virtual DbSet<TeacherCourse> TeacherCourses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=labdb;Username=postgres;Password=1234;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Accounts_pkey");

            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.Email)
                .HasColumnName("Email");

            entity.Property(e => e.IdentityUserId)
                .HasColumnName("IdentityUserId");

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("UQ_Accounts_Email");

            entity.HasIndex(e => e.IdentityUserId)
                .IsUnique()
                .HasDatabaseName("UQ_Accounts_IdentityUserId");

            entity.Property(e => e.SystemMessage)
                .HasColumnName("SystemMessage");

            entity.Property(e => e.HasUnreadSystemMessage)
                .HasColumnName("HasUnreadSystemMessage");
        });

        modelBuilder.Entity<AccountScourse>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.ScoreId }).HasName("Account_Scourses_pkey");

            entity.ToTable("Account_Scourses");

            entity.Property(e => e.AccountId).HasColumnName("AccountId");
            entity.Property(e => e.ScoreId).HasColumnName("ScoreId");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountScourses)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AccountFk");

            entity.HasOne(d => d.Score).WithMany(p => p.AccountScourses)
                .HasForeignKey(d => d.ScoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ScoreFk");
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Certificates_pkey");

            entity.Property(e => e.Id).UseIdentityColumn();

            entity.HasIndex(e => new { e.AccountId, e.CourseId })
                .IsUnique()
                .HasDatabaseName("UQ_Certificates_Account_Course");

            entity.HasOne(d => d.Account).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("AccountFk");

            entity.HasOne(d => d.Course).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("CourseFk");

            entity.Property(e => e.IssuedDate)
                .HasColumnName("IssuedDate")
                .HasColumnType("timestamp without time zone");
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
            entity.HasKey(e => new { e.CourseId, e.AccountId }).HasName("Course_Accounts_pkey");

            entity.ToTable("Course_Accounts");

            entity.Property(e => e.CourseId).HasColumnName("CourseId");
            entity.Property(e => e.AccountId).HasColumnName("AccountId");

            entity.Property(e => e.SubscribedAt)
                .HasColumnName("SubscribedAt")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Account).WithMany(p => p.CourseAccounts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Accounts_AccountId_fkey");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseAccounts)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Course_Accounts_CourseId_fkey");
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

        modelBuilder.Entity<TeacherRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TeacherRequests_pkey");

            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.IdentityUserId)
                .IsRequired()
                .HasColumnName("IdentityUserId");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Name");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasColumnName("Email");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("Status");

            entity.Property(e => e.RequestedAt)
                .HasColumnName("RequestedAt")
                .HasColumnType("timestamp without time zone");

            entity.Property(e => e.AdminMessage)
                .HasColumnName("AdminMessage");
        });

        modelBuilder.Entity<TeacherRequestCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TeacherRequestCourses_pkey");

            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.TeacherRequestId)
                .HasColumnName("TeacherRequestId");

            entity.Property(e => e.CourseId)
                .HasColumnName("CourseId");

            entity.HasOne(d => d.TeacherRequest)
                .WithMany(p => p.TeacherRequestCourses)
                .HasForeignKey(d => d.TeacherRequestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeacherRequestCourses_TeacherRequests");

            entity.HasOne(d => d.Course)
                .WithMany(p => p.TeacherRequestCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeacherRequestCourses_Courses");
        });

        modelBuilder.Entity<TeacherCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TeacherCourses_pkey");

            entity.Property(e => e.Id).UseIdentityColumn();

            entity.Property(e => e.AccountId)
                .HasColumnName("AccountId");

            entity.Property(e => e.CourseId)
                .HasColumnName("CourseId");

            entity.HasOne(d => d.Account)
                .WithMany(p => p.TeacherCourses)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeacherCourses_Accounts");

            entity.HasOne(d => d.Course)
                .WithMany(p => p.TeacherCourses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeacherCourses_Courses");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}