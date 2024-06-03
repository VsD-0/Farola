using System;
using System.Collections.Generic;

namespace Farola.Database.Models;

/// <summary>
/// Токены обновления
/// </summary>
public partial class RefreshToken
{
    /// <summary>
    /// Идентификатор токена
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public int Userid { get; set; }

    /// <summary>
    /// Токен
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// Дата и время создания
    /// </summary>
    public DateTime Createdat { get; set; }

    /// <summary>
    /// Дата и время истечения срока действия
    /// </summary>
    public DateTime Expiresat { get; set; }

    public virtual User User { get; set; } = null!;
}
