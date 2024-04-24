using Microsoft.EntityFrameworkCore;

namespace Farola.Database.Models;

public partial class FarolaContext : DbContext
{
    public FarolaContext()
    {
    }

    public FarolaContext(DbContextOptions<FarolaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Statement> Statements { get; set; }

    public virtual DbSet<StatementStatus> StatementStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reviews_pkey");

            entity.ToTable("reviews");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.StatementId).HasColumnName("statement_id");
            entity.Property(e => e.Text).HasColumnName("text");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("specializations_pkey");

            entity.ToTable("specializations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Statement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("statements_pkey");

            entity.ToTable("statements");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Client).HasColumnName("client");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("now()")
                .HasColumnName("date_added");
            entity.Property(e => e.Professional).HasColumnName("professional");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.ClientNavigation).WithMany(p => p.StatementClientNavigations)
                .HasForeignKey(d => d.Client)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_statement_client");

            entity.HasOne(d => d.ProfessionalNavigation).WithMany(p => p.StatementProfessionalNavigations)
                .HasForeignKey(d => d.Professional)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_statement_professional");

            entity.HasOne(d => d.Status).WithMany(p => p.Statements)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_statement_status");
        });

        modelBuilder.Entity<StatementStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("statement_status_pkey");

            entity.ToTable("statement_statuses");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('statement_status_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(40)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Area)
                .HasMaxLength(50)
                .HasColumnName("area");
            entity.Property(e => e.DateRegistration)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_registration");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Information).HasColumnName("information");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .HasColumnName("phone_number");
            entity.Property(e => e.Photo)
                .HasMaxLength(80)
                .HasColumnName("photo");
            entity.Property(e => e.Profession)
                .HasMaxLength(100)
                .HasColumnName("profession");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Specialization).HasColumnName("specialization");
            entity.Property(e => e.Surname)
                .HasMaxLength(80)
                .HasColumnName("surname");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_role");

            entity.HasOne(d => d.SpecializationNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Specialization)
                .HasConstraintName("fk_user_specialization");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
