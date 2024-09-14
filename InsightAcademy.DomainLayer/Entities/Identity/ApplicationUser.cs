using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        public string Website { get; set; }
        public string TimeZone { get; set; }
        public string StreetAdress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime LastOnline { get; set; }
        public List<Chat> MessagesSent { get; set; }
        public List<Chat> MessagesReceived { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool Status { get; set; }

        [NotMapped]
        public string FullName => FirstName + " " + LastName;

        [NotMapped]
        public IFormFile UserImage { get; set; }
    }
}