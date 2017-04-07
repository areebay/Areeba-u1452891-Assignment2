using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web;

namespace CastleApp.Models
{
    public class Booking
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "From")]
        public DateTime FromDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "To")]
        public DateTime ToDate { get; set; }

        [Required]
        [Display(Name = "User")]
        public string userId { get; set; }

        public Booking()
        {
            userId = HttpContext.Current.User.Identity.GetUserId();
        }
    }

    public class BookingDBContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; }
    }
}