namespace API.SignalR
{
    public class PresenceTracker
    {
        // one username (key) with many connectionId (value) as user might logged in from laptop/phone
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId) {
            // use lock() in case there's many users connected in the same time
            // they need to wait for their turn while app try to add connectionId
            lock(OnlineUsers) {
                if (OnlineUsers.ContainsKey(username)) {
                    OnlineUsers[username].Add(connectionId);
                }
                else {
                    OnlineUsers.Add(username, new List<string>{connectionId});
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId) {
            lock(OnlineUsers) {
                if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;

                // remove connectionId from that particular user
                OnlineUsers[username].Remove(connectionId);

                // remove user from dictionary if there is no more connectionId
                if (OnlineUsers[username].Count == 0) {
                    OnlineUsers.Remove(username);
                }
            }

            return Task.CompletedTask;
        }

        // return an array of online users
        public Task<string[]> GetOnlineUsers() {
            string[] onlineUsers;

            lock(OnlineUsers) {
                onlineUsers = OnlineUsers
                    .OrderBy(k => k.Key)
                    .Select(k => k.Key)
                    .ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
    }
}