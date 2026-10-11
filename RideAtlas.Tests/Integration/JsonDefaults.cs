using System.Text.Json;
using System.Text.Json.Serialization;

namespace RideAtlas.Tests.Integration;

public static class JsonDefaults
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}
