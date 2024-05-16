using System.Collections.Generic;

namespace communication
{
    public class EAGLE_EVENT_Message
    {
        public Message message { get; set; } 
        public EAGLE_EVENT_Message_Data data { get; set; }
    }

    public class EAGLE_EVENT_Message_Data
    {
        public string playerName { get; set; }
        public List<PlayerState> playerStates { get; set; }
    }
}