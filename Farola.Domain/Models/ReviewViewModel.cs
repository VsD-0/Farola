using Farola.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farola.Domain.Models
{
    public class ReviewViewModel
    {
        public User Professional {  get; set; }
        public float AvgGrade { get; set; }
        public int CountGrade { get; set; }
        public User Client { get; set; }
        public float Grade { get; set; }
        public string? Text { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
