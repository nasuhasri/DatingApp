using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

            if (isOnline) {
                // Client: used to invoke methods on the clients connected to this hub
                // send message to other users to notify them somebody is now online
                // Others: everybody else except for client that is connecting
                // so when this user connect, anybody else that connected to this same hub is gonna receive username that has just connected
                // sending notification using clients others
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            }

            // get a list of current online users
            var currentUsers = await _tracker.GetOnlineUsers();

            // send the info to only connected clients when somebody connects
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            if (isOffline) {
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}