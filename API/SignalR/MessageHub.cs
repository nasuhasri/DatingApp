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

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
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

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) {
                var group = GetGroupName(sender.UserName, receipent.UserName);
                await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        // get group name with 2 username in alphabetical order
        private string GetGroupName(string caller, string other) {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}