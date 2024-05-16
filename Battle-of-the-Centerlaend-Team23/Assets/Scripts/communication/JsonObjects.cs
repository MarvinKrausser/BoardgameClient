using System.IO;
using Newtonsoft.Json;

namespace communication
{
    /// <summary>
    /// This class reads in Json files and returns the parsed objects. For test purpose only. 
    /// </summary>
    public class JsonObjects
    {
        public static BoardConfig getBoardConfig()
        {
            string str = File.ReadAllText("Assets/Scripts/Examples/boardconfig.json");
            return JsonConvert.DeserializeObject<BoardConfig>(str);
        }

        public static PARTICIPANTS_INFO_Message getParticipantsInfo()
        {
            string str = File.ReadAllText("Assets/Scripts/Examples/global/participantsInfo.json");
            return JsonConvert.DeserializeObject<PARTICIPANTS_INFO_Message>(str);
        }

        public static GAME_STATE_Message getGameState()
        {
            string str = File.ReadAllText("Assets/Scripts/Examples/ingame/gameState.json");
            return JsonConvert.DeserializeObject<GAME_STATE_Message>(str);
        }

        public static RIVER_EVENT_Message getRiverEvent()
        {
            string str = File.ReadAllText("Assets/Scripts/Examples/ingame/riverEvent.json");
            return JsonConvert.DeserializeObject<RIVER_EVENT_Message>(str);
        }
        
        public static SHOT_EVENT_Message getShotEvent()
        {
            string str = File.ReadAllText("Assets/Scripts/Examples/ingame/shotEvent.json");
            return JsonConvert.DeserializeObject<SHOT_EVENT_Message>(str);
        }
        
        public static CARD_EVENT_Message getCardEvent()
        {
            string str = File.ReadAllText("Assets/Scripts/Examples/ingame/cardEvent.json");
            return JsonConvert.DeserializeObject<CARD_EVENT_Message>(str);
        }
    }
}