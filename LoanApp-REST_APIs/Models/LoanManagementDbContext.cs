using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_REST_APIs.Models;

public partial class LoanManagementDbContext : DbContext
{
    public LoanManagementDbContext()
    {
    }

    public LoanManagementDbContext(DbContextOptions<LoanManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BackgroundVerificationAssignment> BackgroundVerificationAssignments { get; set; }

    public virtual DbSet<FeedbackAnswer> FeedbackAnswers { get; set; }

    public virtual DbSet<FeedbackQuestion> FeedbackQuestions { get; set; }

    public virtual DbSet<FeedbackSubmission> FeedbackSubmissions { get; set; }

    public virtual DbSet<Help> Helps { get; set; }

    public virtual DbSet<LoanRequest> LoanRequests { get; set; }

    public virtual DbSet<LoanVerificationAssignment> LoanVerificationAssignments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.; Database = LoanManagementDB; Trusted_Connection=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BackgroundVerificationAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Backgrou__DA8918146BFCE2BC");

            entity.ToTable("Background_Verification_Assignments");

            entity.HasIndex(e => e.RequestId, "UQ__Backgrou__18D3B90E10FBD91C").IsUnique();

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("assigned_at");
            entity.Property(e => e.OfficerId).HasColumnName("officer_id");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("assigned")
                .HasColumnName("status");
            entity.Property(e => e.VerificationDetails)
                .HasColumnType("text")
                .HasColumnName("verification_details");

            entity.HasOne(d => d.Officer).WithMany(p => p.BackgroundVerificationAssignments)
                .HasForeignKey(d => d.OfficerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Backgroun__offic__4AB81AF0");

            entity.HasOne(d => d.Request).WithOne(p => p.BackgroundVerificationAssignment)
                .HasForeignKey<BackgroundVerificationAssignment>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Backgroun__reque__49C3F6B7");
        });

        modelBuilder.Entity<FeedbackAnswer>(entity =>
        {
            entity.HasKey(e => e.AnswerId).HasName("PK__Feedback__3372431821B24536");

            entity.ToTable("Feedback_Answers");

            entity.HasIndex(e => new { e.SubmissionId, e.QuestionId }, "UQ_Submission_Question").IsUnique();

            entity.Property(e => e.AnswerId).HasColumnName("answer_id");
            entity.Property(e => e.Answer)
                .HasColumnType("text")
                .HasColumnName("answer");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");

            entity.HasOne(d => d.Question).WithMany(p => p.FeedbackAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback___quest__6477ECF3");

            entity.HasOne(d => d.Submission).WithMany(p => p.FeedbackAnswers)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback___submi__6383C8BA");
        });

        modelBuilder.Entity<FeedbackQuestion>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Feedback__2EC215495E50A4C3");

            entity.ToTable("Feedback_Questions");

            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.QuestionText)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("question_text");
        });

        modelBuilder.Entity<FeedbackSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Feedback__9B535595F6D98571");

            entity.ToTable("Feedback_Submissions");

            entity.Property(e => e.SubmissionId).HasColumnName("submission_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.SubmissionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("submission_date");

            entity.HasOne(d => d.Customer).WithMany(p => p.FeedbackSubmissions)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback___custo__5EBF139D");
        });

        modelBuilder.Entity<Help>(entity =>
        {
            entity.HasKey(e => e.HelpId).HasName("PK__Help__D20BFE65DE5068AC");

            entity.ToTable("Help");

            entity.Property(e => e.HelpId).HasColumnName("help_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<LoanRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Loan_Req__18D3B90F021A7EB7");

            entity.ToTable("Loan_Requests");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.AssignedOfficerId).HasColumnName("assigned_officer_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Details)
                .HasColumnType("text")
                .HasColumnName("details");
            entity.Property(e => e.LoanType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("loan_type");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("request_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("pending")
                .HasColumnName("status");

            entity.HasOne(d => d.AssignedOfficer).WithMany(p => p.LoanRequestAssignedOfficers)
                .HasForeignKey(d => d.AssignedOfficerId)
                .HasConstraintName("FK__Loan_Requ__assig__4222D4EF");

            entity.HasOne(d => d.Customer).WithMany(p => p.LoanRequestCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Loan_Requ__custo__412EB0B6");
        });

        modelBuilder.Entity<LoanVerificationAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Loan_Ver__DA8918144EC5D308");

            entity.ToTable("Loan_Verification_Assignments");

            entity.HasIndex(e => e.RequestId, "UQ__Loan_Ver__18D3B90E04BE6271").IsUnique();

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("assigned_at");
            entity.Property(e => e.OfficerId).HasColumnName("officer_id");
            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("assigned")
                .HasColumnName("status");
            entity.Property(e => e.VerificationDetails)
                .HasColumnType("text")
                .HasColumnName("verification_details");

            entity.HasOne(d => d.Officer).WithMany(p => p.LoanVerificationAssignments)
                .HasForeignKey(d => d.OfficerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Loan_Veri__offic__52593CB8");

            entity.HasOne(d => d.Request).WithOne(p => p.LoanVerificationAssignment)
                .HasForeignKey<LoanVerificationAssignment>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Loan_Veri__reque__5165187F");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC5792E417");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__783254B1541EC8B2").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FB05E3479");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616443E5A338").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC5729345FEE9").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("mobile_number");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__role_id__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
