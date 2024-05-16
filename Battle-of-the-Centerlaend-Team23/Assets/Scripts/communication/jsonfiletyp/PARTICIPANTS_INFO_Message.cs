namespace communication
{
    public class PARTICIPANTS_INFO_Message
    {
        public Message message { get; set; }
        public PARTICIPANTS_INFO_Message_Data data { get; set; }
    }

    public class PARTICIPANTS_INFO_Message_Data
    {
        public string [] players { get; set; }
        public string [] spectators { get; set; }
        public string [] ais { get; set; }
        public string [] readyPlayers { get; set; }
    }
}