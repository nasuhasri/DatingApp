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
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(IUnitOfWork uow, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            _uow = uow;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            // get the name of the user we are connected to
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _uow.MessageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            if (_uow.HasChanges()) await _uow.Complete();

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto) {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.ReceipentUsername.ToLower()) {
                throw new HubException("You cannot send messages to yourself!");
            }

            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var receipent = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.ReceipentUsername);

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
            var group = await _uow.MessageRepository.GetMessageGroup(groupName);

            // check connections and see if we have username that matches the receipent's username
            if (group.Connections.Any(x => x.Username == receipent.UserName)) {
                // mark message as read
                message.DateRead = DateTime.UtcNow;
            } 
            else {
                var connections = await PresenceTracker.GetConnectionsForUser(receipent.UserName);
                if (connections != null) {
                    // users connected to our application
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new {username = sender.UserName, knownAs = sender.KnownAs});
                }
            }

            _uow.MessageRepository.AddMessage(message);

            if (await _uow.Complete()) {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        // get group name with 2 username in alphabetical order
        private string GetGroupName(string caller, string other) {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        // add group to db
        private async Task<Group> AddToGroup(string groupName) {
            var group = await _uow.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null) {
                group = new Group(groupName);
                _uow.MessageRepository.AddGroup(group);
            }

            // add connection to that group
            group.Connections.Add(connection);

            if (await _uow.Complete()) return group;

            throw new HubException("Failed to add to group!");
        }

        // remove connection from db
        private async Task<Group> RemoveMessageGroup() {
            var group = await _uow.MessageRepository.GetGroupForConnectionId(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            _uow.MessageRepository.RemoveConnection(connection);

            if (await _uow.Complete()) return group;

            throw new HubException("Failed to remove from group!");
        }
    }
}