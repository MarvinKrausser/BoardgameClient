using System;
using System.Collections.Generic;

namespace communication
{
    public class HELLO_CLIENT_Message
    {
        public Message message { get; set; }
        public HELLO_CLIENT_Message_Data data { get; set; }
    }
    
    public class HELLO_CLIENT_Message_Data
    {
        public Guid reconnectToken { get; set; }  //"Guid" stands for UUID
        public BoardConfig boardConfig { get; set; }
        public GameConfig gameConfig { get; set; }
    }
}