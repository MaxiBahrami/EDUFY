using InsightAcademy.DomainLayer.Entities.Identity;
using InsightAcademy.DomainLayer.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.Entities
{
    public class Chat : BaseEntity
    {
        public string SenderId { get; set; }
        public string SenderUserName { get; set; }
        public string SenderUserProfileUrl { get; set; }
        public ApplicationUser Sender { get; set; }
        public string RecipientId { get; set; }
        public string RecipientUserName { get; set; }
        public string RecipientUserProfileUrl { get; set; }
        public ApplicationUser Recipient { get; set; }
        public string Message { get; set; }
        public DateTime? DateRead { get; set; }
        public bool IsRead { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        public MessageTypeEnum Type { get; set; }
        public string FileName { get; set; }
        public string File { get; set; }
        public string Extension { get; set; }
        public string Size { get; set; }
        public string GroupName { get; set; }

        [NotMapped]
        public string Base64File { get; set; }
    }
}