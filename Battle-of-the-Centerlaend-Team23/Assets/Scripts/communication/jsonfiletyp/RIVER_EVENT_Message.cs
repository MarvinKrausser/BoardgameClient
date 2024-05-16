using System;
using System.Collections.Generic;

namespace communication
{
    public class RIVER_EVENT_Message
    {
        public Message message { get; set; }
        public RIVER_EVENT_Message_Data data { get; set; }
    }

    public class RIVER_EVENT_Message_Data
    {
        public string playerName { get; set; }
        public PlayerState[][] playerStates { get; set; }
        public BoardState[] boardStates { get; set; }
    }
}