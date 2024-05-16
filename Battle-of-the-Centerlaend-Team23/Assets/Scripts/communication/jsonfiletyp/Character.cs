using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace communication
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Character
    {
        FRODO,
        SAM,
        LEGOLAS,
        GIMLI,
        GANDALF,
        ARAGORN,
        GOLLUM,
        GALADRIEL,
        BOROMIR,
        BAUMBART,
        MERRY,
        PIPPIN,
        ARWEN
    }
}