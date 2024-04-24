namespace Farola.Domain.Models
{
    /// <summary>
    /// Информацию о пагинациии.
    /// </summary>
    /// <typeparam name="T">Тип элементов, для которых применяется пагинация.</typeparam>
    public class Pagination<T>
    {
        /// <summary>
        /// Номер текущей страницы.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Размер страницы.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Количество элементов в результирующем наборе.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Общее количество страниц в результирующем наборе.
        /// </summary>
        public int TotalPages
        {
            get { return (int)Math.Ceiling((double)TotalItems / PageSize); }
        }
    }
}
