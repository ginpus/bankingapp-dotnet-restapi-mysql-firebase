using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Enums
{
    public enum TransactionType
    {
        Unknown = 0,
        Debit = 1,
        Credit = 2,
        TopUp = 3,
        WithDraw = 4
    }
}
