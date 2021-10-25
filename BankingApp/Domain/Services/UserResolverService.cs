using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class UserResolverService : IUserResolverService
    {
        private readonly IHttpContextAccessor _context;
        public UserResolverService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public ClaimsPrincipal User => _context.HttpContext?.User;

        public string UserId => _context.HttpContext?.User?.Claims?.SingleOrDefault(claim => claim.Type == "user_id").Value;

        public IHeaderDictionary HeaderDictionary => _context.HttpContext?.Request?.Headers;

        public string IdToken => _context.HttpContext?.Request?.Headers?.FirstOrDefault(key => key.Key == "Authorization").Value;
    }
}
