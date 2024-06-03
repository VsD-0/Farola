using System;
using System.Collections.Generic;

namespace Farola.Database.Models;

/// <summary>
/// Таблица пользователей системы
/// </summary>
public partial class User
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Номер роли
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Номер телефона
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Электронная почта
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Пароль
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Место работы
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// Подробная информация
    /// </summary>
    public string? Information { get; set; }

    /// <summary>
    /// Номер cпециализация
    /// </summary>
    public int? SpecializationId { get; set; }

    /// <summary>
    /// Имя фото
    /// </summary>
    public string? Photo { get; set; }

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime DateRegistration { get; set; }

    /// <summary>
    /// Профессия
    /// </summary>
    public string? Profession { get; set; }

    /// <summary>
    /// Отчество
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    /// Токен обновления
    /// </summary>
    public string? RefreshToken { get; set; }

    public virtual ICollection<Favorite> FavoriteClients { get; set; } = new List<Favorite>();

    public virtual ICollection<Favorite> FavoriteProfessionals { get; set; } = new List<Favorite>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Role Role { get; set; } = null!;

    public virtual Specialization? Specialization { get; set; }

    public virtual ICollection<Statement> StatementClients { get; set; } = new List<Statement>();

    public virtual ICollection<Statement> StatementProfessionals { get; set; } = new List<Statement>();
}
