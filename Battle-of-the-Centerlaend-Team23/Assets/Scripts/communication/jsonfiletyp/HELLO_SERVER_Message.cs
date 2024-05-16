namespace communication
{
    public class HELLO_SERVER_Message
    {
        public Message message { get; set; }
        public HELLO_SERVER_Message_Data data { get; set; }
    }

    public class HELLO_SERVER_Message_Data
    {
        public Role role { get; set; }
        public string name { get; set; }
    }
}