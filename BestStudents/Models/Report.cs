using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BestStudents.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public int Money { get; set; }
        public string Description { get; set; }
    }
}