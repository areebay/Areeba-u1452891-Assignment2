using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CastleApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CastleApp.Controllers
{
    public class BookingsController : Controller
    {
        private BookingDBContext db = new BookingDBContext();
        public string cal = String.Empty;
        private int selectedRoomNumber;

        // GET: Bookings
        public ActionResult Index()
        {
            // replace userIds with user names for display
            var udb = new ApplicationDbContext();
            List<Booking> bookings = db.Bookings.ToList();

            foreach(Booking booking in bookings)
            {
                var user = udb.Users.Find(booking.userId);
                booking.userId = user.UserName;
            }

            return View(db.Bookings.ToList().OrderBy(x => x.RoomNumber).ThenBy(x => x.FromDate));
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // dynamically create javascript for jquery calendar
        // uses snippets from: http://stackoverflow.com/questions/25179979/asp-net-mvc5-display-data-in-calendar
        private void SetCalendar()
        {
            cal += "<script>" + "\n";
            cal += "var $j = jQuery.noConflict();" + "\n";
            cal += "$j(document).ready(function() {" + "\n";
            cal += "  var SelectedDates = { };" + "\n";

            // get bookings
            List<Booking> bookings = db.Bookings.ToList();

            foreach (Booking booking in bookings)
            {
                if (selectedRoomNumber == booking.RoomNumber)
                {
                    // get all dates for current booking
                    DateTime from = booking.FromDate;
                    DateTime to = booking.ToDate;

                    for (var dt = from; dt <= to; dt = dt.AddDays(1))
                    {
                        cal += "  SelectedDates[new Date('" + dt.Date.ToString("MM/dd/yyyy") + "')] = new Date('" + dt.Date.ToString("MM/dd/yyyy") + "').toString();" + "\n";
                    }
                }
            }

            cal += "  var newDt = new Date();" + "\n";
            cal += "  var year = newDt.getFullYear(); // Current Year" + "\n";
            cal += "  $j('#dvCalendar').datepicker({" + "\n";
            cal += "    defaultDate: new Date((parseInt(year, 10) - 1), 12, 31)," + "\n";
            cal += "    numberOfMonths: [2, 6]," + "\n";
            cal += "    changeMonth: false," + "\n";
            cal += "    changeYear: false," + "\n";
            cal += "    duration: 'fast'," + "\n";
            cal += "    stepMonths: 0," + "\n";
            cal += "    beforeShowDay: function(date) {" + "\n";
            cal += "      var Highlight = SelectedDates[date];" + "\n";
            cal += "      if (Highlight)" + "\n";
            cal += "      {" + "\n";
            cal += "        return [true, 'Highlighted', Highlight];" + "\n";
            cal += "      }" + "\n";
            cal += "      else" + "\n";
            cal += "      {" + "\n";
            cal += "        return [true, '', ''];" + "\n";
            cal += "      }" + "\n";
            cal += "    }" + "\n";
            cal += "  });" + "\n";
            cal += "});" + "\n";
            cal += "</script>";

            ViewData["cal"] = cal;
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            //db.Database.ExecuteSqlCommand("DELETE FROM Bookings");  
            int.TryParse(Request.QueryString["RoomNumber"], out selectedRoomNumber);
            if (selectedRoomNumber == 0)
            {
                selectedRoomNumber = 1;
            }

            // create list of room numbers for dropdown
            List<SelectListItem> roomNums = new List<SelectListItem>() {
                new SelectListItem() { Text = "1", Value = "1" },
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" }
            };

            ViewBag.RoomNumbersList = new SelectList(roomNums, "Value", "Text", selectedRoomNumber);

            SetCalendar();

            // Audit bookings view
            Audit audit = new Audit();
            audit.Date = DateTime.Now;
            audit.UserId = User.Identity.GetUserId();
            audit.Event = "User view bookings: " + User.Identity.GetUserName() + " (Room:" + selectedRoomNumber + ")";
            var audit_result = new AuditsController().Create(audit);

            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RoomNumber,FromDate,ToDate,userId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.userId = User.Identity.GetUserId();
                db.Bookings.Add(booking);
                db.SaveChanges();

                // Audit booking submitted
                Audit audit = new Audit();
                audit.Date = DateTime.Now;
                audit.UserId = User.Identity.GetUserId();
                audit.Event = "User booking submitted: " + User.Identity.GetUserName() + " (Room:" + booking.RoomNumber + ")";
                var audit_result = new AuditsController().Create(audit);

                return RedirectToAction("Details/" + booking.ID);
            }

            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RoomNumber,FromDate,ToDate,userId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
