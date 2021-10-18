using System.Text.Json.Serialization;

namespace Domain.Client.Models.ResponseModels
{
    public class ReturnUserResponse
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("expiresIn")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("localId")]
        public string LocalId { get; set; }
    }
}
