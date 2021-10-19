using Persistence.Models.ReadModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public interface IUsersRepository
    {
        Task<UserReadModel> GetUserAsync(string localId);

        Task<int> CreateUserAysnc(UserWriteModel user);

        Task<IEnumerable<UserReadModel>> GetAllUsersAsync();

        Task<int> EditEmailAsync(Guid userId, string newEmail);
    }
}
