using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace communication
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Card
    {
        MOVE_3,
        MOVE_2,
        MOVE_1,
        MOVE_BACK,
        U_TURN,
        RIGHT_TURN,
        LEFT_TURN,
        AGAIN,
        LEMBAS,
        EMPTY
    }
}