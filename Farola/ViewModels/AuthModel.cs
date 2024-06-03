namespace Farola.ViewModels
{
    public class AuthModel
    {
        /// <summary>
        /// Электронная почта
        /// </summary>
        /// <example>ivan.ivanov@gmail.com</example>
        public string? Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        /// <example>Ivan123#</example>
        public string? Password { get; set; }

        /// <summary>
        /// Токен Обновления
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}
