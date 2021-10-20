﻿using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Models.ReadModels
{
    public class TransactionReadModel
    {
        public Guid TransactionId { get; set; }
        public string Iban { get; set; }
        public TransactionType Type { get; set; }
        public decimal Sum { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
    }
}
