namespace communication
{
    public class PAUSE_REQUEST_Message
    {
        public Message message { get; set; }
        public PAUSE_REQUEST_Message_Data data { get; set; }
    }

    public class PAUSE_REQUEST_Message_Data
    {
        public bool pause { get; set; }
    }
}