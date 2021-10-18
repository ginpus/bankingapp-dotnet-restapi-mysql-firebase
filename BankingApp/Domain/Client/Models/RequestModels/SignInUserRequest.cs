using System.Text.Json.Serialization;

namespace Domain.Client.Models.RequestModels
{
    public class SignInUserRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
