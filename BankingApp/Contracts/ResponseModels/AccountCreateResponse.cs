using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.ResponseModels
{
    public class AccountCreateResponse
    {
        public string Iban { get; set; }

        public decimal Balance { get; set; }

        public Guid UserId { get; set; }
    }
}
