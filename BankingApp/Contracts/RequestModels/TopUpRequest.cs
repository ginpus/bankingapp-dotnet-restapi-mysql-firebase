using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Contracts.RequestModels
{
    public class TopUpRequest
    {
        [JsonPropertyName("iban")]
        public string Iban { get; set; }

        [JsonPropertyName("sum")]
        public decimal Sum { get; set; }
    }
}
