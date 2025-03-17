using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BestStudents.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
    }
}