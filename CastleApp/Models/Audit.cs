using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace CastleApp.Models
{
    public class Audit
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "User Id")]
        public string UserId { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        public string Event { get; set; }
    }

    public class AuditDBContext : DbContext
    {
        public DbSet<Audit> AuditLog { get; set; }
    }
}