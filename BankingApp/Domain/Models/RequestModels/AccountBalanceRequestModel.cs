using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.RequestModels
{
    public class AccountBalanceRequestModel
    {
        public string Iban { get; set; }

        public Guid UserId { get; set; }
    }
}
