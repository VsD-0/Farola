using Farola.Database.Models;

namespace Farola.Infrastructure.Models
{
    public static class CurrentUser
    {
        public static string Id { get; set; }
        public static string Role { get; set; }
        public static string UserName { get; set; }
    }
}
