using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            // return latest message
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch {
                "Inbox" => query.Where(u => u.ReceipentUsername == messageParams.Username),
                // messages sent by that particular user
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username),
                // unread messages
                _ => query.Where(u => u.ReceipentUsername == messageParams.Username && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>
                .CreateAsync(messages, messageParams.pageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string receipentUserName)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Receipent).ThenInclude(p => p.Photos)
                .Where(
                    m => (m.ReceipentUsername == currentUserName && m.SenderUsername == receipentUserName) || (m.ReceipentUsername == receipentUserName && m.SenderUsername == currentUserName)
                )
                .OrderByDescending(m => m.MessageSent) // get latest message first
                .ToListAsync();

            // get a list of unread messages and mark them as sent
            var unreadMessages = messages.Where(m => m.DateRead == null && m.ReceipentUsername == currentUserName).ToList();

            if (unreadMessages.Any()) {
                foreach (var message in unreadMessages) {
                    // mark message as read once received by receipent
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}