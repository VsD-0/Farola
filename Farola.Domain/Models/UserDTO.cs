namespace Farola.Domain.Models
{
    public class UserDTO
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
        /// Фамилия
        /// </summary>
        public string Surname { get; set; } = null!;

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Отчество
        /// </summary>
        public string? Patronymic { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; } = null!;

        /// <summary>
        /// Почта
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Место работы
        /// </summary>
        public string? Area { get; set; }

        /// <summary>
        /// Подробная информация
        /// </summary>
        public string? Information { get; set; }

        /// <summary>
        /// Специализация
        /// </summary>
        public string? Specialization { get; set; }

        /// <summary>
        /// Фото
        /// </summary>
        public string? Photo { get; set; }

        /// <summary>
        /// Профессия
        /// </summary>
        public string? Profession { get; set; }
    }
}
