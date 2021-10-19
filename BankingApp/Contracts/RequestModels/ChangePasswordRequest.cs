using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Contracts.RequestModels
{
    public class ChangePasswordRequest
    {
        [JsonPropertyName("password")]
        public string NewPassword { get; set; }
    }
}
