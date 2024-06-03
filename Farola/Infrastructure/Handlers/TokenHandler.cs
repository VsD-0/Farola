using Farola.API;
using Farola.Database.Models;
using Farola.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace Farola.Infrastructure.Handlers
{
    public class TokenStorage
    {
        public static string? Token { get; set; }
    }
    /// <summary>
    /// Обработчик токена для добавления и обновления JWT-токена перед отправкой запросов.
    /// </summary>
    public class TokenHandler : DelegatingHandler
    {
        #region Fields
        private readonly IUserClient _authData;
        #endregion Fields

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TokenHandler"/>.
        /// </summary>
        /// <param name="authData">Сервис аутентификации для обновления токена.</param>
        public TokenHandler(IUserClient authData)
        {
            _authData = authData;
        }

        /// <summary>
        /// Добавляет или обновляет JWT-токен перед отправкой запроса.
        /// </summary>
        /// <param name="request">Запрос, который будет отправлен.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Ответ от сервера.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(TokenStorage.Token))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(TokenStorage.Token);

                if (DateTime.UtcNow > jwtToken.ValidTo)
                {
                    try
                    {
                        int userId = int.Parse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid")?.Value ?? throw new("Тип не был найден"));
                        User user = await _authData.GetUserById(userId);

                        if (user == null)
                        {
                            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        }

                        var newToken = await _authData.SignIn(new AuthModel 
                        {
                            Email = user?.Email,
                            Password = user?.Password, 
                            RefreshToken = user?.RefreshToken });
                        TokenStorage.Token = newToken;
                    }
                    catch (Exception ex)
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenStorage.Token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
