using System;
using System.Collections.Generic;

namespace Farola.Database.Models;

/// <summary>
/// Таблица отзывов клиентов
/// </summary>
public partial class Review
{
    /// <summary>
    /// Идентификатор отзыва
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Номер заявления
    /// </summary>
    public int StatementId { get; set; }

    /// <summary>
    /// Оценка работы
    /// </summary>
    public float Grade { get; set; }

    /// <summary>
    /// Текст отзыва
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Дата добавления
    /// </summary>
    public DateTime DateAdded { get; set; }

    public virtual Statement Statement { get; set; } = null!;
}
