namespace communication
{
    public class CARD_OFFER_Message
    {
        public Message message { get; set; } 
        public CARD_OFFER_Message_Data data { get; set; }
    }

    public class CARD_OFFER_Message_Data
    {
        public Card [] cards { get; set; }
    }
}