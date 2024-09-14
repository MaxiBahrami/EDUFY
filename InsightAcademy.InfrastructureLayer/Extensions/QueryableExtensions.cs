using InsightAcademy.DomainLayer.Entities;

namespace InsightAcademy.UI.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<Chat> MarkUnreadAsRead(this IQueryable<Chat> query, string currentUsername)
        {
            var unreadMessages = query.Where(m => m.DateRead == null
                && m.RecipientUserName == currentUsername);

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return query;
        }
    }
}