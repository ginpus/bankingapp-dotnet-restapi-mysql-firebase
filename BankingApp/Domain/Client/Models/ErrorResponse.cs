using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Client.Models
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }
}
