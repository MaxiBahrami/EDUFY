

namespace InsightAcademy.ApplicationLayer.Dtos
{
    public class ChatDto
    {
        public string SenderId { get; set; }
        public string SenderUserName { get; set; }
        public string SenderPhotoUrl { get; set; }
        public string RecipientId { get; set; }
        public string RecipientUserName { get; set; }
        public string RecipientPhotoUrl{ get; set; }

        public string Message { get; set; }
        public DateTime? DateRead { get; set; }
      
    }
}
