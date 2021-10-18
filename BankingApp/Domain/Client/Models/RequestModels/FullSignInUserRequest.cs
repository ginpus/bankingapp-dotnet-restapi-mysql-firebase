using System.Text.Json.Serialization;

namespace Domain.Client.Models.RequestModels
{
    public class FullSignInUserRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("returnSecureToken")]
        public bool ReturnSecureToken { get; set; }
    }
}
