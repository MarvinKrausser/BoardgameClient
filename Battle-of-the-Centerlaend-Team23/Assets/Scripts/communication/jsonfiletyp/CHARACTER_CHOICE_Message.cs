namespace communication
{
    public class CHARACTER_CHOICE_Message
    {
        public Message message { get; set; } 
        public CHARACTER_CHOICE_Message_Data data { get; set; }
    }

    public class CHARACTER_CHOICE_Message_Data
    {
        public Character characterChoice { get; set; }
    }
}