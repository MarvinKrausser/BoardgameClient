using System.Collections.Generic;

namespace communication
{
    public class CARD_EVENT_Message
    {
        public Message message { get; set; } 
        public CARD_EVENT_Message_Data data { get; set; }
    }

    public class CARD_EVENT_Message_Data
    {
        public string playerName { get; set; }
        public Card card { get; set; }
        public List<PlayerState> [] playerStates { get; set; }
        public List<BoardState> boardStates { get; set; }
    }
}