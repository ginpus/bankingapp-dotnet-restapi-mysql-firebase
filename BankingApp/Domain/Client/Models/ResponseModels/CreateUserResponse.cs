using System.Text.Json.Serialization;

namespace Domain.Client.Models.ResponseModels
{
    public class CreateUserResponse
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

        public override string ToString()
        {
            return $"idToken: {IdToken} \n email: {Email} \n refreshToken:{RefreshToken} \n expiresIn: {ExpiresIn} \n localId: {LocalId}";
        }
    }
}