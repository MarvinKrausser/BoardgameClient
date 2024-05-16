using System;

namespace communication
{
    public class RECONNECT_Message
    {
        public Message message { get; set; } 
        public RECONNECT_Message_Data data { get; set; }
    }

    public class RECONNECT_Message_Data
    {
        public string name { get; set; }
        public Guid reconnectToken { get; set; } //"Guid" stands for UUID
    }
}