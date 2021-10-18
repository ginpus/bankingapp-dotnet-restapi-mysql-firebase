using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.RequestModels
{
    public class ChangePasswordRequestModel
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }

        [JsonPropertyName("password")]
        public string NewPassword { get; set; }

        [JsonPropertyName("returnSecureToken")]
        public bool ReturnSecureToken { get; set; }

    }
}
