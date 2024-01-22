namespace API.SignalR
{
    public class PresenceTracker
    {
        // one username (key) with many connectionId (value) as user might logged in from laptop/phone
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string username, string connectionId) {
            bool isOnline = false;

            // use lock() in case there's many users connected in the same time
            // they need to wait for their turn while app try to add connectionId
            lock(OnlineUsers) {
                // users are already online and we just add connectionId
                if (OnlineUsers.ContainsKey(username)) {
                    OnlineUsers[username].Add(connectionId);
                }
                else {
                    // users just recently online
                    OnlineUsers.Add(username, new List<string>{connectionId});
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId) {
            bool isOffline = false;

            lock(OnlineUsers) {
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                // remove connectionId from that particular user
                OnlineUsers[username].Remove(connectionId);

                // remove user from dictionary if there is no more connectionId
                if (OnlineUsers[username].Count == 0) {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
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

        public static Task<List<string>> GetConnectionsForUser(string username) {
            List<string> connectionIds;

            lock(OnlineUsers) {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }
}