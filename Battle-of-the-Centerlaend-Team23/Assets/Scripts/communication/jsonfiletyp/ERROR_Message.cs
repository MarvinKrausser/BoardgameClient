namespace communication
{
    public class ERROR_Message
    {
        public Message message { get; set; }
        public ERROR_Message_Data data { get; set; }
    }

    public class ERROR_Message_Data
    {
        public int errorCode { get; set; }
        public string reason { get; set; }
    }
}