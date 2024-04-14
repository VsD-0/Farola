using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farola.Domain.Models
{
    /// <summary>
    /// Пагинация
    /// </summary>
    /// <typeparam name="T">Тип элементов в результирующей коллекции.</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Элементы результирующей коллекции.
        /// </summary>
        public List<T>? Items { get; set; }

        /// <summary>
        /// Информацию о пагинации.
        /// </summary>
        public Pagination<T>? Pagination { get; set; }
    }
}
