namespace communication
{
    public class PAUSED_Message
    {
        public Message message { get; set; }
        public PAUSED_Message_Data data { get; set; }
    }

    public class PAUSED_Message_Data
    {
        public string playerName { get; set; }
        public bool paused { get; set; }
    }
}