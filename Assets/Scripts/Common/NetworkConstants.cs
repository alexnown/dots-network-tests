using UnityEngine;

namespace com.testnet.common
{
    public class NetworkConstants
    {
        public const string DEFAULT_SERVER_LOBBY_IP = "127.0.0.1";
        public const ushort DEFAULT_SERVER_LOBBY_PORT = 7979;

        public static string SERVER_LOBBY_IP = DEFAULT_SERVER_LOBBY_IP;
        public static ushort SERVER_LOBBY_PORT = DEFAULT_SERVER_LOBBY_PORT;
        public static ushort SERVER_REALTIME_ROOM_PORT { get => (ushort)(SERVER_LOBBY_PORT + 1); }
    }
}
