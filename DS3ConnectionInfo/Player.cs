using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;

namespace DS3ConnectionInfo
{
    public class Player
    {
        private static Dictionary<CSteamID, Player> activePlayers = new Dictionary<CSteamID, Player>();

        public string SteamName { get; private set; }
        public ulong SteamId64 { get; private set; }
        public bool IsRelay { get; private set; }
        public string RelayText => IsRelay ? "Yes" : "";

        private Player(CSteamID steamID, P2PSessionState_t session)
        {
            SteamName = SteamFriends.GetFriendPersonaName(steamID);
            SteamId64 = steamID.m_SteamID;
            IsRelay = session.m_bUsingRelay != 0;
        }

        public static IEnumerable<Player> ActivePlayers()
        {
            return activePlayers.Values.AsEnumerable();
        }

        public static void UpdatePlayerList()
        {
            activePlayers.Clear();

            try
            {
                int count = SteamFriends.GetCoplayFriendCount();
                for (int i = 0; i < count; i++)
                {
                    CSteamID id = SteamFriends.GetCoplayFriend(i);

                    P2PSessionState_t session = new P2PSessionState_t();
                    if (!SteamNetworking.GetP2PSessionState(id, out session))
                        continue;
                    if (session.m_bConnectionActive == 0 && session.m_bConnecting == 0)
                        continue;

                    activePlayers[id] = new Player(id, session);
                }
            }
            catch { }
        }
    }
}
