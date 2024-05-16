using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace communication
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Message
    {
        ERROR,
        INVALID_MESSAGE,
        PARTICIPANTS_INFO,
        HELLO_SERVER,
        HELLO_CLIENT,
        GOODBYE_SERVER,
        PLAYER_READY,
        GAME_START,
        GAME_END,
        RECONNECT,
        CHARACTER_OFFER,
        CHARACTER_CHOICE,
        CARD_OFFER,
        CARD_CHOICE,
        ROUND_START,
        CARD_EVENT,
        SHOT_EVENT,
        RIVER_EVENT,
        EAGLE_EVENT,
        GAME_STATE,
        PAUSE_REQUEST,
        PAUSED
    }
}