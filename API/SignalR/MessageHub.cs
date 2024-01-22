using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            // get the name of the user we are connected to
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto) {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.ReceipentUsername.ToLower()) {
                throw new HubException("You cannot send messages to yourself!");
            }

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var receipent = await _userRepository.GetUserByUsernameAsync(createMessageDto.ReceipentUsername);

            if (receipent == null) throw new HubException("Not found user!");

            var message = new Message {
                // EF knows that sender has sender.id since it is AppUser object and it automatically set it but only for the Id
                Sender = sender,
                Receipent = receipent,
                SenderUsername = sender.UserName,
                ReceipentUsername = receipent.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, receipent.UserName);
            var group = await _messageRepository.GetMessageGroup(groupName);

            // check connections and see if we have username that matches the receipent's username
            if (group.Connections.Any(x => x.Username == receipent.UserName)) {
                // mark message as read
                message.DateRead = DateTime.UtcNow;
            }

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        // get group name with 2 username in alphabetical order
        private string GetGroupName(string caller, string other) {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        // add group to db
        private async Task<bool> AddToGroup(string groupName) {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null) {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            // add connection to that group
            group.Connections.Add(connection);

            return await _messageRepository.SaveAllAsync();
        }

        // remove connection from db
        private async Task RemoveMessageGroup() {
            var connection = await _messageRepository.GetConnection(Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            await _messageRepository.SaveAllAsync();
        }
    }
}