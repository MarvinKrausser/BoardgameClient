using System.Collections.Generic;

namespace communication
{
    public class GAME_STATE_Message
    {
        public Message message { get; set; } 
        public GAME_STATE_Message_Data data { get; set; }
    }

    public class GAME_STATE_Message_Data
    {
        public List<PlayerState> playerStates { get; set; }
        public BoardState boardState { get; set; }
        public int currentRound { get; set; }
    }
}