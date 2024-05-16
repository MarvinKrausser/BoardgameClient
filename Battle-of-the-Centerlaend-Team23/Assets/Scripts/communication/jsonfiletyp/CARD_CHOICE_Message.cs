namespace communication
{
    public class CARD_CHOICE_Message
    {
        public Message message { get; set; }
        public CARD_CHOICE_Message_Data data { get; set; }
    }

    public class CARD_CHOICE_Message_Data
    {
        public Card [] cards { get; set; }
    }
}