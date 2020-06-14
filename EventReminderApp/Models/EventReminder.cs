using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventReminderApp.Models
{
    public class EventReminder
    {
        public int EventID { get; set; }
        public int UserID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
             
    }
}