using InsightAcademy.DomainLayer.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsightAcademy.DomainLayer.Entities
{
    public class Tutor : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        [Required]
        [StringLength(500)]
        public string Tagline { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        public decimal HourlyRate { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string ZipCode { get; set; }

        [StringLength(500)]
        public string Language { get; set; }

        [Required]
        [StringLength(500)]
        [ValidateNever]
        public string Services { get; set; } = "MyHome";

        [Required]
        [StringLength(1000)]
        public string Introduction { get; set; }

        public string GoogleMeetLink { get; set; }

        public string EmailAddress { get; set; }

        public bool IsVerified { get; set; }
        public bool IsBlock { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Contact Contact { get; set; }
        public List<Education> Education { get; set; }
        public List<TutorSubject> TutorSubject { get; set; }
        public List<Like> Likes { get; set; }
        public List<Media> Media { get; set; }

        [NotMapped]
        public string FullName => FirstName + " " + LastName;

        [NotMapped]
        public List<string> SelectedServices { get; set; }

        public List<WishList> WishLists { get; set; }

        public IEnumerable<Review> Reviews { get; set; }
    }
}