using System;
using System.Collections.Generic;

namespace Farola.Database.Models;

/// <summary>
/// Справочник специализаций
/// </summary>
public partial class Specialization
{
    /// <summary>
    /// Идентификатор специализации
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Наименование специализации
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Фото специализации
    /// </summary>
    public string? Photo { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
