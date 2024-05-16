using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;

namespace communication
{
    public class GAME_END_Message
    {
        public Message message { get; set; }
        public GAME_END_Message_Data data { get; set; }
    }

    public class GAME_END_Message_Data
    {
        public List<PlayerState> playerStates { get; set; }
        public string winner { get; set; }
        [CanBeNull] public List<Additional> additional { get; set; }
    }

    public class Additional
    {
        public string name { get; set; }
        public string value { get; set; }
        public string description { get; set; }
    }
}