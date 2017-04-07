using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace CastleApp.Models
{
    public class Feedback
    {
        public int ID { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        public string Comment { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public int Rating { get; set; }

        public Feedback()
        {
            Date = DateTime.Now;
        }
    }

    public class FeedbackDBContext : DbContext
    {
        public DbSet<Feedback> Feedback { get; set; }
    }
}