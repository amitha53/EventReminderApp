using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventReminderApp.Models
{
    public class Registeration
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}