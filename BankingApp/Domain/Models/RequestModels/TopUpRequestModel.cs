using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.RequestModels
{
    public class TopUpRequestModel
    {
        public string Iban { get; set; }
        public decimal Sum { get; set; }
        public Guid UserId { get; set; }
    }
}
