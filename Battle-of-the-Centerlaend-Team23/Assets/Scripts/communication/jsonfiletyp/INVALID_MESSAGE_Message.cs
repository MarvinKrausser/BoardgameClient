namespace communication
{
    public class INVALID_MESSAGE_Message
    {
        public Message message { get; set; }
        public INVALID_MESSAGE_Message_Data data { get; set; }
    }

    public class INVALID_MESSAGE_Message_Data
    {
        public string invalidMessage { get; set; }
    }
}