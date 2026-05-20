using ikincelwebsitesi.Models;

namespace ikincelwebsitesi.Interfaces
{
    public interface IMessageService
    {
        Task<List<Message>> GetInboxAsync(string userId);
        Task<List<Message>> GetSentMessagesAsync(string userId);
        Task<Message?> GetMessageByIdAsync(int id);
        Task SendMessageAsync(Message message);
        Task MarkAsReadAsync(int messageId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<List<Message>> GetChatHistoryAsync(string user1Id, string user2Id, int listingId);
    }
}
