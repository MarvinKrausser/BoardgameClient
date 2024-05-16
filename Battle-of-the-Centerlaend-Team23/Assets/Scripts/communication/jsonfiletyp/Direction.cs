using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace communication
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Direction
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }
}