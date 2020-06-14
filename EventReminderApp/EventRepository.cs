using EventReminderApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EventReminderApp
{
    public class EventRepository
    {
        const string Connectionstring = @"Data Source= LENOVO\SQLSERVER; Initial Catalog = dbwebapp; Integrated Security = True";

        SqlConnection con = new SqlConnection(Connectionstring);

        public void AddEditEvent(EventReminder events, string userid)
        {
            string qry = string.Empty;
            var startDate = Convert.ToDateTime(events.StartDate);
            var endDate = Convert.ToDateTime(events.EndDate);
            if (events.EventID > 0)
            {
                qry = "Update tblEvents set Subject = '" + events.Subject + "', Description = '" + events.Description +
                       "', StartDate= '" + startDate + "',EndDate= '" + endDate + "' where EventID= '" + events.EventID + "' and UserID= " + userid;
            }
            else
            {

                qry = "insert into tblEvents(UserID,Subject,Description,StartDate,EndDate)" +
                    " values('" + userid + "','" + events.Subject + "','" + events.Description + "','" + startDate + "','" + endDate + "')";
            }
            this.AddUpdateDeleteSQL(qry);
        }

        public int AddUpdateDeleteSQL(string qry)
        {
            con.Open();
            int count = new SqlCommand(qry, con).ExecuteNonQuery();
            con.Close();
            return count;
        }

        public EventReminder GetEventById(int eventId)
        {
            string qry = "select * from tblEvents where EventId=" + eventId;
            DataRow row = GetSQLList(qry).Rows[0];

            return new EventReminder
            {
                EventID = Convert.ToInt32(row.ItemArray[0]),
                UserID = Convert.ToInt32(row.ItemArray[1]),
                Subject = row.ItemArray[2].ToString(),
                Description = row.ItemArray[3].ToString(),
                StartDate = row.ItemArray[4].ToString(),
                EndDate = row.ItemArray[5].ToString(),
            };
        }

        private DataTable GetSQLList(string qry)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader sdr = cmd.ExecuteReader();
            DataTable datatable = new DataTable();
            datatable.Load(sdr);
            con.Close();
            return datatable;
        }

        public int DeleteEvent(int eventId)
        {
            string qry = "select * from tblEvents where EventId=" + eventId;
            return this.AddUpdateDeleteSQL(qry);
        }

        public List<EventReminder> EventsList(string userid)
        {
            string query = "Select * from tblEvents where UserID=" + userid;
            DataTable datatable = GetSQLList(query);

            List<EventReminder> eventList = new List<EventReminder>();

            foreach (DataRow row in datatable.Rows)
            {
                EventReminder eventModel = new EventReminder();
                eventModel.EventID = Convert.ToInt32(row.ItemArray[0]);
                eventModel.UserID = Convert.ToInt32(row.ItemArray[1]);
                eventModel.Subject = row.ItemArray[2].ToString();
                eventModel.Description = row.ItemArray[3].ToString();
                eventModel.StartDate = row.ItemArray[4].ToString();
                eventModel.EndDate = row.ItemArray[5].ToString();
                eventList.Add(eventModel);
            }
            return eventList;
        }



    }
}