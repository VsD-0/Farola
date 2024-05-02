namespace Farola.Database.Models;

/// <summary>
/// Таблица избранных специалистов
/// </summary>
public partial class Favorite
{
    /// <summary>
    /// Идентификатор
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

    public virtual User Client { get; set; } = null!;

    public virtual User Professional { get; set; } = null!;
}
