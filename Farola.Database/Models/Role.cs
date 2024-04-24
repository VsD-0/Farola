namespace Farola.Database.Models;

/// <summary>
/// Справочник ролей пользователей
/// </summary>
public partial class Role
{
    /// <summary>
    /// Идентификатор роли
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Наименование роли
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
