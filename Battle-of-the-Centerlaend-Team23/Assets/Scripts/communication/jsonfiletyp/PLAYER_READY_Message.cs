namespace communication
{
    public class PLAYER_READY_Message
    {
        public Message message { get; set; }
        public PLAYER_READY_Message_Data data { get; set; }
    }

    public class PLAYER_READY_Message_Data
    {
        public bool ready { get; set; }
    }
}