using System.Collections.Generic;

namespace communication
{
    public class ROUND_START_Message
    {
        public Message message { get; set; } 
        public ROUND_START_Message_Data data { get; set; }
    }

    public class ROUND_START_Message_Data
    {
        public List<PlayerState> playerStates { get; set; }
    }
}