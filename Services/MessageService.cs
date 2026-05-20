using ikincelwebsitesi.Data;
using ikincelwebsitesi.Interfaces;
using ikincelwebsitesi.Models;
using Microsoft.EntityFrameworkCore;

namespace ikincelwebsitesi.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetInboxAsync(string userId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Listing)
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<List<Message>> GetSentMessagesAsync(string userId)
        {
            return await _context.Messages
                .Include(m => m.Receiver)
                .Include(m => m.Listing)
                .Where(m => m.SenderId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.Listing)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task SendMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(int messageId)
        {
            var msg = await _context.Messages.FindAsync(messageId);
            if (msg != null)
            {
                msg.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }

        public async Task<List<Message>> GetChatHistoryAsync(string user1Id, string user2Id, int listingId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.Listing)
                .Where(m => m.ListingId == listingId && 
                           ((m.SenderId == user1Id && m.ReceiverId == user2Id) || 
                            (m.SenderId == user2Id && m.ReceiverId == user1Id)))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
