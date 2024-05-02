using Microsoft.AspNetCore.Mvc;

namespace Farola.API.Infrastructure.Exceptions
{
    public class CustomDetails : ProblemDetails
    {
        public string AdditionalInfo { get; set; }
    }
    public class CustomException : Exception
    {
        public string AdditionalInfo { get; set; }
        public string Type { get; set; }
        public string Detail { get; set; }
        public string Title { get; set; }
        public string Instance { get; set; }

        public CustomException(string instance)
        {
            Type = "custom-exception";
            Detail = "Произошла ошибка на стороне сервера.";
            Title = "Внутренняя ошибка сервера";
            AdditionalInfo = "";
            Instance = instance;
        }
    }
}
