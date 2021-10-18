using System.Text.Json.Serialization;

namespace Domain.Client.Models.ResponseModels
{
    public class ClientSignInUserResponse
    {
        [JsonPropertyName("localId")]
        public string LocalId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }

    }
}
