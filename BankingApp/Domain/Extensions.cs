using Domain.Client.Models.ResponseModels;
using Domain.Models.RequestModels;
using Domain.Models.ResponseModels;
using Persistence.Models.WriteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public static class Extensions
    {
        public static UserWriteModel AsDto(this UserResponseModel user)
        {
            return new UserWriteModel
            {
                UserId = user.UserId,
                Email = user.Email,
                LocalId = user.LocalId,
                DateCreated = user.DateCreated
            };
        }

        public static UserResponseModel AsDto(this UserReadModel user)
        {
            return new UserResponseModel
            {
                UserId = user.UserId,
                Email = user.Email,
                LocalId = user.LocalId,
                DateCreated = user.DateCreated
            };
        }


        public static EditUserResponse AsDto(this ChangePasswordOrEmailResponse user)
        {
            return new EditUserResponse
            {
                Email = user.Email,
                LocalId = user.LocalId,
                IdToken = user.IdToken
            };
        }



        public static ChangePasswordOrEmailResponse AsDto(this ClientChangePasswordOrEmailResponse user)
        {
            return new ChangePasswordOrEmailResponse
            {
                Email = user.Email,
                LocalId = user.LocalId,
                IdToken = user.IdToken
            };
        }
    }
}
