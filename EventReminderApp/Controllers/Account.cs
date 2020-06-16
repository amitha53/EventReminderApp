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
    public class AccountController : Controller
    {
        string Connectionstring = @"Data Source= LENOVO\SQLSERVER; Initial Catalog = dbwebapp; Integrated Security = True";
        EventRepository eventRepository = new EventRepository();
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
            string userid = null;
            if (Session["UserID"] != null)
            {
                userid = Session["UserID"].ToString();
            }
            List<EventReminder> eventlist = eventRepository.EventsList(userid);
            return new JsonResult { Data = eventlist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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
                eventRepository.AddUpdateDeleteSQL(qry);
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
                eventRepository.AddUpdateDeleteSQL(qry);
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        public ActionResult Create()
        {
            ViewBag.pageload = "create";
            return View(new EventReminder());
        }
        [HttpPost]
        public ActionResult Create(EventReminder events)
        {
            string userid = Session["UserID"].ToString();
            eventRepository.AddEditEvent(events, userid);
            return RedirectToAction("Calenderlist", "Account");
        }

        public ActionResult Edit(int id)
        {
            EventReminder events = eventRepository.GetEventById(id);
            return View("Create", events);

        }

        [HttpPost]
        public ActionResult Edit(EventReminder events)
        {
            string userid = Session["UserID"].ToString();
            eventRepository.AddEditEvent(events, userid);
            ViewBag.pageload = "edit";
            
            return RedirectToAction("ListEvents", "Account");
        }

        public ActionResult Delete(int id)
        {
            eventRepository.DeleteEvent(id);
            return RedirectToAction("ListEvents", "Account");
        }
        public ActionResult ListEvents()
        {
            string userid = Session["UserID"].ToString();
            List<EventReminder> eventlist = eventRepository.EventsList( userid);
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
                string query = "Select UserID,EmailId,Password From tblRegister Where EmailId=@EmailId and Password=@Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@EmailId", login.Email);
                cmd.Parameters.AddWithValue("@Password", login.Password);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable datatable = new DataTable();
                sda.Fill(datatable);
                if (datatable.Rows.Count==1)
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

        [HttpPost]
        public JsonResult GoogleLogin(string email, string name, string gender, string lastname, string location)
        {
            var status = false;
            using (SqlConnection con = new SqlConnection(Connectionstring))
            {
                string qry;
                con.Open();
                string query = $"Select UserID,EmailId from tblRegister where EmailId='{email}' ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable datatable = new DataTable();
                sda.Fill(datatable);
                if (datatable.Rows.Count == 1)
                {
                    DataRow row = datatable.Rows[0];
                    string uid = row["UserID"].ToString();
                    string mail = row["EmailId"].ToString();
                    Session["UserID"] = uid;
                    Session["EmailId"] = mail;

                    status = true;
                }
                else
                {
                    qry = "insert into tblRegister(UserName,EmailId)" +
                    " values('" + name + "','" + email + "')";
                    eventRepository.AddUpdateDeleteSQL(qry);
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.ClearHeaders();
            return RedirectToAction("Loginform", "Account");
        }
    }
}
