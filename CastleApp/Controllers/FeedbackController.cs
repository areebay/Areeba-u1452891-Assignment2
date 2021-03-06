﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CastleApp.Models;
using Microsoft.AspNet.Identity;

namespace CastleApp.Controllers
{
    public class FeedbackController : Controller
    {
        private FeedbackDBContext db = new FeedbackDBContext();

        // GET: /Feedback/
        public ActionResult Index(string client, string comment)
        {
            // define LINQ query for search
            var results = from m in db.Feedback
                          select m;

            bool search = false;

            // filter by Client if client search string has been specified
            if (!String.IsNullOrEmpty(client))
            {
                results = results.Where(s => s.ClientName.Contains(client));
                search = true;
            }

            // filter by Comments if comment search string has been specified
            if (!String.IsNullOrEmpty(comment))
            {
                results = results.Where(s => s.Comment.Contains(comment));
                search = true;                
            }

            // Audit (only if a user is logged-in)
            if (User.Identity.GetUserId() != null && User.Identity.GetUserId() != String.Empty)
            {
                Audit audit = new Audit();
                audit.Date = DateTime.Now;
                audit.UserId = User.Identity.GetUserId();
                audit.Event = "User search: " + User.Identity.GetUserName() +
                    " (ClientSearchVal: " + client + ")" +
                    " (CommentSearchVal: " + comment + ")";
                var audit_result = new AuditsController().Create(audit);
            }

            if (search)
            {
                return View(results);
            }

            return View(db.Feedback.ToList());
        }

        // GET: /Feedback/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedback.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // GET: /Feedback/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Feedback/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,ClientName,Comment,Date,Rating")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.Date = DateTime.Now;
                db.Feedback.Add(feedback);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(feedback);
        }

        // GET: /Feedback/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedback.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // POST: /Feedback/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,ClientName,Comment,Date,Rating")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(feedback);
        }

        // GET: /Feedback/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedback.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // POST: /Feedback/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feedback feedback = db.Feedback.Find(id);
            db.Feedback.Remove(feedback);
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
