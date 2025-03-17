using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BestStudents.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int Money { get; set; }
        public int RoleId { get; set; }
    }
}