using Farola.Database.Models;
using Microsoft.FluentUI.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farola.Domain.Models
{
    public class StatementsViewModel
    {
        public int Id { get; set; }

        public string Client { get; set; }
        public User Pro {  get; set; }

        public string phoneClient { get; set; }

        public string Status { get; set; }
        
        public float? Grade {  get; set; }

        public float? GradeByClient { get; set; }

        public float? AvgGradeClient { get; set; }

        public float? AvgGradePro { get; set; }

        public Option<string> GradeOption { get; set; }

        public List<Option<string>> Grades { get; set; }

        public string Review { get; set; }

        public string Comment { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime? DateExpiration { get; set; }
    }
}
