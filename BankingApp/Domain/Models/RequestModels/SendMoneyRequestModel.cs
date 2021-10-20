using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.RequestModels
{
    public class SendMoneyRequestModel
    {
        public string SenderIban { get; set; }

        public decimal Sum { get; set; }

        public string ReceiverIban { get; set; }

        public Guid UserId { get; set; }
    }
}
