namespace communication
{
    public class CHARACTER_OFFER_Message
    {
        public Message message { get; set; } 
        public CHARACTER_OFFER_Message_Data data { get; set; }
    }

    public class CHARACTER_OFFER_Message_Data
    {
        public Character [] characters { get; set; } 
    }
}