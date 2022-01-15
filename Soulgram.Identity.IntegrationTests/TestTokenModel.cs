using Newtonsoft.Json;

namespace Soulgram.Identity.IntegrationTests;

public record TestTokenModel
{
    [JsonProperty("access_token")]
    public string AccessToken { get; init; }

    [JsonProperty("token_type")]
    public string TokenType { get; init; }
}