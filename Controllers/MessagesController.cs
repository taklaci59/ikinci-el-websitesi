using ikincelwebsitesi.Interfaces;
using ikincelwebsitesi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ikincelwebsitesi.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly IListingService _listingService;

        public MessagesController(IMessageService messageService, IListingService listingService)
        {
            _messageService = messageService;
            _listingService = listingService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var inbox = await _messageService.GetInboxAsync(userId!);
            var sent = await _messageService.GetSentMessagesAsync(userId!);

            // Combine and group by conversation (OtherUser + Listing)
            var allMessages = inbox.Concat(sent).ToList();
            
            var conversations = allMessages
                .GroupBy(m => new { 
                    OtherId = m.SenderId == userId ? m.ReceiverId : m.SenderId, 
                    m.ListingId 
                })
                .Select(g => g.OrderByDescending(m => m.SentAt).First())
                .OrderByDescending(m => m.SentAt)
                .ToList();

            return View(conversations);
        }

        public async Task<IActionResult> Sent()
        {
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var initialMessage = await _messageService.GetMessageByIdAsync(id);

            if (initialMessage == null || (initialMessage.SenderId != userId && initialMessage.ReceiverId != userId))
                return Forbid();

            var otherUserId = initialMessage.SenderId == userId ? initialMessage.ReceiverId : initialMessage.SenderId;
            var history = await _messageService.GetChatHistoryAsync(userId!, otherUserId, initialMessage.ListingId);

            // Mark all messages from the other user as read
            foreach (var msg in history.Where(m => m.ReceiverId == userId && !m.IsRead))
            {
                await _messageService.MarkAsReadAsync(msg.Id);
            }

            ViewBag.CurrentUserId = userId;
            ViewBag.Listing = initialMessage.Listing;
            ViewBag.OtherUser = initialMessage.SenderId == userId ? initialMessage.Receiver : initialMessage.Sender;

            return View(history);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(int listingId, string content, string? receiverId = null)
        {
            var listing = await _listingService.GetListingByIdAsync(listingId);
            if (listing == null) return NotFound();

            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // If receiverId is not provided, we assume we are starting a chat with the listing owner
            var finalReceiverId = receiverId ?? listing.UserId;

            if (senderId == finalReceiverId && string.IsNullOrEmpty(receiverId)) 
            {
                return BadRequest("Kendi ilanınıza mesaj gönderemezsiniz.");
            }

            var message = new Message
            {
                ListingId = listingId,
                SenderId = senderId!,
                ReceiverId = finalReceiverId,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            await _messageService.SendMessageAsync(message);

            // If it's a reply from the chat page, stay on the chat page
            if (Request.Headers["Referer"].ToString().Contains("/Messages/Details"))
            {
                return RedirectToAction(nameof(Details), new { id = message.Id });
            }

            return RedirectToAction("Details", "Listings", new { id = listingId, messageSent = true });
        }
    }
}
