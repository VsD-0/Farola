using System;
using System.Collections.Generic;
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

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Statement> Statements { get; set; }

    public virtual DbSet<StatementStatus> StatementStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("favorites_pkey");

            entity.ToTable("favorites", tb => tb.HasComment("Таблица избранных специалистов"));

            entity.Property(e => e.Id)
                .HasComment("Идентификатор")
                .HasColumnName("id");
            entity.Property(e => e.ClientId)
                .HasComment("Номер клиента")
                .HasColumnName("client_id");
            entity.Property(e => e.ProfessionalId)
                .HasComment("Номер специалиста")
                .HasColumnName("professional_id");

            entity.HasOne(d => d.Client).WithMany(p => p.FavoriteClients)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_favorite_client");

            entity.HasOne(d => d.Professional).WithMany(p => p.FavoriteProfessionals)
                .HasForeignKey(d => d.ProfessionalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_favorite_professional");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refreshtokens_pkey");

            entity.ToTable("refresh_tokens", tb => tb.HasComment("Токены обновления"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('refreshtokens_id_seq'::regclass)")
                .HasComment("Идентификатор токена")
                .HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Дата и время создания")
                .HasColumnName("createdat");
            entity.Property(e => e.Expiresat)
                .HasComment("Дата и время истечения срока действия")
                .HasColumnName("expiresat");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .HasComment("Токен")
                .HasColumnName("token");
            entity.Property(e => e.Userid)
                .HasComment("Идентификатор пользователя")
                .HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_token_user");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reviews_pkey");

            entity.ToTable("reviews", tb => tb.HasComment("Таблица отзывов клиентов"));

            entity.Property(e => e.Id)
                .HasComment("Идентификатор отзыва")
                .HasColumnName("id");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("now()")
                .HasComment("Дата добавления")
                .HasColumnName("date_added");
            entity.Property(e => e.Grade)
                .HasComment("Оценка работы")
                .HasColumnName("grade");
            entity.Property(e => e.StatementId)
                .HasComment("Номер заявления")
                .HasColumnName("statement_id");
            entity.Property(e => e.Text)
                .HasComment("Текст отзыва")
                .HasColumnName("text");

            entity.HasOne(d => d.Statement).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.StatementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_review_statement");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles", tb => tb.HasComment("Справочник ролей пользователей"));

            entity.Property(e => e.Id)
                .HasComment("Идентификатор роли")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasComment("Наименование роли")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("specializations_pkey");

            entity.ToTable("specializations", tb => tb.HasComment("Справочник специализаций"));

            entity.Property(e => e.Id)
                .HasComment("Идентификатор специализации")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasComment("Наименование специализации")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Statement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("statements_pkey");

            entity.ToTable("statements", tb => tb.HasComment("Таблица заявлений"));

            entity.Property(e => e.Id)
                .HasComment("Идентификатор заявления")
                .HasColumnName("id");
            entity.Property(e => e.ClientId)
                .HasComment("Номер клиента")
                .HasColumnName("client_id");
            entity.Property(e => e.Comment)
                .HasComment("Комментарий специалиста")
                .HasColumnName("comment");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("now()")
                .HasComment("Дата создания")
                .HasColumnName("date_added");
            entity.Property(e => e.DateExpiration)
                .HasComment("Дата закрытия заявки")
                .HasColumnName("date_expiration");
            entity.Property(e => e.Grade)
                .HasComment("Оценка специалиста на заказ")
                .HasColumnName("grade");
            entity.Property(e => e.ProfessionalId)
                .HasComment("Номер специалиста")
                .HasColumnName("professional_id");
            entity.Property(e => e.StatusId)
                .HasComment("Номер статуса заявления")
                .HasColumnName("status_id");

            entity.HasOne(d => d.Client).WithMany(p => p.StatementClients)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_statement_client");

            entity.HasOne(d => d.Professional).WithMany(p => p.StatementProfessionals)
                .HasForeignKey(d => d.ProfessionalId)
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

            entity.ToTable("statement_statuses", tb => tb.HasComment("Справочник статусов заявлений"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('statement_status_id_seq'::regclass)")
                .HasComment("Идентификатор статуса заявления")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(40)
                .HasComment("Наименование статуса")
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", tb => tb.HasComment("Таблица пользователей системы"));

            entity.Property(e => e.Id)
                .HasComment("Идентификатор пользователя")
                .HasColumnName("id");
            entity.Property(e => e.Area)
                .HasMaxLength(50)
                .HasComment("Место работы")
                .HasColumnName("area");
            entity.Property(e => e.DateRegistration)
                .HasDefaultValueSql("now()")
                .HasComment("Дата регистрации")
                .HasColumnName("date_registration");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasComment("Электронная почта")
                .HasColumnName("email");
            entity.Property(e => e.Information)
                .HasComment("Подробная информация")
                .HasColumnName("information");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasComment("Имя пользователя")
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasComment("Пароль")
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(80)
                .HasComment("Отчество")
                .HasColumnName("patronymic");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(12)
                .HasComment("Номер телефона")
                .HasColumnName("phone_number");
            entity.Property(e => e.Photo)
                .HasMaxLength(80)
                .HasComment("Имя фото")
                .HasColumnName("photo");
            entity.Property(e => e.Profession)
                .HasMaxLength(100)
                .HasComment("Профессия")
                .HasColumnName("profession");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .HasComment("Токен обновления");
            entity.Property(e => e.RoleId)
                .HasComment("Номер роли")
                .HasColumnName("role_id");
            entity.Property(e => e.SpecializationId)
                .HasComment("Номер cпециализация")
                .HasColumnName("specialization_id");
            entity.Property(e => e.Surname)
                .HasMaxLength(100)
                .HasComment("Фамилия пользователя")
                .HasColumnName("surname");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_role");

            entity.HasOne(d => d.Specialization).WithMany(p => p.Users)
                .HasForeignKey(d => d.SpecializationId)
                .HasConstraintName("fk_user_specialization");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
