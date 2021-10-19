using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Models.WriteModels
{
    public class AccountWriteModel
    {
        public string Iban { get; set; }

        public decimal Balance { get; set; }

        public Guid UserId { get; set; }
    }
}
