using System;
using System.Collections.Generic;

namespace Farola.Database.Models;

/// <summary>
/// Таблица заявлений
/// </summary>
public partial class Statement
{
    /// <summary>
    /// Идентификатор заявления
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Номер специалиста
    /// </summary>
    public int ProfessionalId { get; set; }

    /// <summary>
    /// Номер клиента
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    /// Номер статуса заявления
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime DateAdded { get; set; }

    /// <summary>
    /// Дата закрытия заявки
    /// </summary>
    public DateTime? DateExpiration { get; set; }

    /// <summary>
    /// Оценка специалиста на заказ
    /// </summary>
    public float? Grade { get; set; }

    /// <summary>
    /// Комментарий специалиста
    /// </summary>
    public string? Comment { get; set; }

    public virtual User Client { get; set; } = null!;

    public virtual User Professional { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual StatementStatus Status { get; set; } = null!;
}
