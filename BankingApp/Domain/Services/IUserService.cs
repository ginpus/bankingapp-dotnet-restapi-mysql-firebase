using Contracts.RequestModels;
using Contracts.ResponseModels;
using Domain.Client.Models.ResponseModels;
using Domain.Models.RequestModels;
using Domain.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IUserService
    {
        Task<UserResponseModel> SignUpAsync(SignUpRequest user);

        Task<UserResponseModel> GetUserAsync(string localId);

        Task<SignInResponse> SignInUserAsync(SignInRequest user);

        Task<ChangePasswordOrEmailResponse> ChangePasswordAsync(ChangePasswordRequestModel request);

        Task<ChangePasswordOrEmailResponse> ChangeEmailAsync(Guid userId, ChangeEmailRequestModel request);
    }
}
