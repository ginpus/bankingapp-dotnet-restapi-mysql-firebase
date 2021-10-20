using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Models.WriteModels
{
    public class AccountSendWriteModel
    {
        public string Iban { get; set; }

        public decimal Balance { get; set; }
    }
}
