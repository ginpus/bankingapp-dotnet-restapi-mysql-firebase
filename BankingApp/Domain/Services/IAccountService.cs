using Contracts.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IAccountService
    {
        Task<int> InsertAccountAsync(AccountCreateResponse newAccount);
    }
}
