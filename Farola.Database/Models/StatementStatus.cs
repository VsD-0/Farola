using System;
using System.Collections.Generic;

namespace Farola.Database.Models;

/// <summary>
/// Справочник статусов заявлений
/// </summary>
public partial class StatementStatus
{
    /// <summary>
    /// Идентификатор статуса заявления
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Наименование статуса
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<Statement> Statements { get; set; } = new List<Statement>();
}
