using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farola.Domain.Models
{
    public class StatementsViewModel
    {
        public string Client { get; set; }

        public string Status { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime? DateExpiration { get; set; }
    }
}
