using System;
using System.Threading;
using managers;

namespace communication
{
    /// <summary>
    /// All global variables are stored in here. Every class can always access them.
    /// </summary>
    public class StaticVariables
    {
        public static string playerName;
        public static BoardConfig boardConfig;
        public static GameConfig gameConfig;
        public static Guid reconnectToken;
        public static PARTICIPANTS_INFO_Message participantsInfoMessage;
        public static GAME_END_Message gameEndMessage;

        public static string ip;
        public static int port;

        public static bool playerIsSpectator;
    }
}