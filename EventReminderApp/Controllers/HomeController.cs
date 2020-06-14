using EventReminderApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EventReminderApp.Controllers
{
    public class HomeController : Controller
    {
        string Connectionstring = @"Data Source= LENOVO\SQLSERVER; Initial Catalog = dbwebapp; Integrated Security = True";

        // GET:  
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Loginform()
        {

            return View();
        }
        public ActionResult Calenderlist()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            List<EventReminder> events = new List<EventReminder>();
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                string userid = Session["UserID"].ToString();
                con.Open();
                string query = "Select * from tblEvents where UserID=" + userid;
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                DataTable datatable = new DataTable();
                datatable.Load(sdr);
                con.Close();

                foreach (DataRow row in datatable.Rows)
                {
                    EventReminder listevents = new EventReminder();
                    listevents.EventID = Convert.ToInt32(row.ItemArray[0]);
                    listevents.UserID = Convert.ToInt32(row.ItemArray[1]);
                    listevents.Subject = row.ItemArray[2].ToString();
                    listevents.Description = row.ItemArray[3].ToString();
                    listevents.StartDate = row.ItemArray[4].ToString();
                    listevents.EndDate = row.ItemArray[5].ToString();
                    events.Add(listevents);
                }
            }
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult SaveEvent(EventReminder events)
        {
            var status = false;
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                string userid = Session["UserID"].ToString();
                con.Open();
                string qry = string.Empty;
                qry = "Update tblEvents set Subject = '" + events.Subject + "', Description = '" + events.Description +
                       "', StartDate= '" + events.StartDate + "',EndDate= '" + events.EndDate + "' where EventID= '" + events.EventID + "' and UserID= " + userid;
                this.AddUpdateDeleteSQL(qry);
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                con.Open();
                EventReminder events = new EventReminder();
                string qry = "delete from tblEvents where EventID=" + eventID;
                this.AddUpdateDeleteSQL(qry);
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        public ActionResult Create()
        {

            return View(new EventReminder());
        }
        [HttpPost]
        public ActionResult Create(EventReminder events)
        {
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                string userid = Session["UserID"].ToString();
                con.Open();
                string qry = string.Empty;
                var startDate = Convert.ToDateTime(events.StartDate);
                var endDate = Convert.ToDateTime(events.EndDate);
                qry = "insert into tblEvents(UserID,Subject,Description,StartDate,EndDate)" +
                    " values('" + userid + "','" + events.Subject + "','" + events.Description + "','" + startDate + "','" + endDate + "')";

                this.AddUpdateDeleteSQL(qry);
            }
            return RedirectToAction("Calenderlist", "Account");
        }
        public EventReminder GetEventId(int eventId)
        {
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                con.Open();
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
        }

        public ActionResult Edit(int id)
        {
            EventReminder events = GetEventId(id);
            return View("Create", events);

        }

        [HttpPost]
        public ActionResult Edit(EventReminder events)
        {
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                string userid = Session["UserID"].ToString();
                con.Open();
                string qry = string.Empty;
                qry = "Update tblEvents set UserID = '" + userid + "', Subject = '" + events.Subject + "', Description = '" + events.Description +
                       "', StartDate= '" + events.StartDate + "',EndDate= '" + events.EndDate + "' where EventID=" + events.EventID;
                this.AddUpdateDeleteSQL(qry);
            }
            return RedirectToAction("ListEvents", "Account");
        }

        public int AddUpdateDeleteSQL(string qry)
        {
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                con.Open();
                int count = new SqlCommand(qry, con).ExecuteNonQuery();
                con.Close();
                return count;
            }
        }

        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                con.Open();
                string qry = "delete from tblEvents where EventID=" + id;
                this.AddUpdateDeleteSQL(qry);
            }
            return RedirectToAction("ListEvents", "Account");
        }
        public ActionResult ListEvents()
        {
            List<EventReminder> eventlist = new List<EventReminder>();
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                string userid = Session["UserID"].ToString();
                con.Open();
                string query = "Select * from tblEvents where UserID=" + userid;
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                DataTable datatable = new DataTable();
                datatable.Load(sdr);
                con.Close();

                foreach (DataRow row in datatable.Rows)
                {
                    EventReminder listevents = new EventReminder();
                    listevents.EventID = Convert.ToInt32(row.ItemArray[0]);
                    listevents.UserID = Convert.ToInt32(row.ItemArray[1]);
                    listevents.Subject = row.ItemArray[2].ToString();
                    listevents.Description = row.ItemArray[3].ToString();
                    listevents.StartDate = row.ItemArray[4].ToString();
                    listevents.EndDate = row.ItemArray[5].ToString();
                    eventlist.Add(listevents);
                }

            }
            return View(eventlist);
        }
        public string Error()
        {

            return ("Invalid Username and Password");
        }
        [HttpPost]
        public ActionResult Register(Registeration register)
        {
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                con.Open();
                string query = "Insert Into tblRegister Values(@UserName,@EmailId,@Password)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserName", register.Username);
                cmd.Parameters.AddWithValue("@EmailId", register.Email);
                cmd.Parameters.AddWithValue("@Password", register.Password);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Loginform", "Account");
        }
        [HttpPost]
        public ActionResult Login(Registeration login)
        {
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                con.Open();
                string query = "Select UserID,@EmailId,Password From tblRegister Where EmailId=@EmailId and Password=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@EmailId", login.Email);
                cmd.Parameters.AddWithValue("@Password", login.Password);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable datatable = new DataTable();
                sda.Fill(datatable);
                if (datatable.Rows.Count == 1)
                {
                    DataRow row = datatable.Rows[0];
                    login.UserID = Convert.ToInt32(row["UserID"]);
                    Session["UserID"] = row["UserID"].ToString();
                    Session["EmailId"] = login.Email.ToString();
                    return RedirectToAction("Calenderlist", "Account");
                }
                else
                {
                    return RedirectToAction("Error", "Account");
                }
            }
        }
        private DataTable GetSQLList(string qry)
        {
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(qry, con);
                SqlDataReader sdr = cmd.ExecuteReader();
                DataTable datatable = new System.Data.DataTable();
                datatable.Load(sdr);
                con.Close();
                return datatable;
            }
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Loginform", "Account");
        }
        /*<div class="form-group">
        <a href = "#" class="btn btn-primary btn-block">
            <i class="icon-facebook"></i>    Login with Facebook
        </a>
                </div>-->
                                            <!--<div class="form-group">
        <a id = "googleLogin" class="btn btn-info btn-block">
            <i class="icon-google"></i>    Login with Google
        </a>
    </div>-->*/
               
    }
}
