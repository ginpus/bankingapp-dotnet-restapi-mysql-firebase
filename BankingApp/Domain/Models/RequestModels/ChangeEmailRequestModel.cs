﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.RequestModels
{
    public class ChangeEmailRequestModel
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }

        [JsonPropertyName("email")]
        public string NewEmail { get; set; }

        [JsonPropertyName("returnSecureToken")]
        public bool ReturnSecureToken { get; set; }
    }
}
