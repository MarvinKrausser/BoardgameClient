using System.Collections.Generic;

namespace communication
{
    public class SHOT_EVENT_Message
    {
        public Message message { get; set; }
        public SHOT_EVENT_Message_Data data { get; set; }
    }

    public class SHOT_EVENT_Message_Data
    {
        public string shooterName { get; set; }
        public string targetName { get; set; }
        public List<PlayerState> playerStates { get; set; }
    }
}