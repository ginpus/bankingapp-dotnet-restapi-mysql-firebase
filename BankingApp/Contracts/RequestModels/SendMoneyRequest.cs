using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Contracts.RequestModels
{
    public class SendMoneyRequest
    {
        [JsonPropertyName("senderIban")]
        public string SenderIban { get; set; }

        [JsonPropertyName("sum")]
        public decimal Sum { get; set; }

        [JsonPropertyName("receiverIban")]
        public string ReceiverIban { get; set; }
    }
}
