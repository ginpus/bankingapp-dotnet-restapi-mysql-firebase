using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Client.Models
{
    public class Error
    {
        [JsonPropertyName("code")]
        public int StatusCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
