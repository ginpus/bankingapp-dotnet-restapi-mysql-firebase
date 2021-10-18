using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Client.Models.ResponseModels
{
    public class ClientChangePasswordOrEmailResponse
    {
        [JsonPropertyName("localId")]
        public string LocalId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }
    }
}
