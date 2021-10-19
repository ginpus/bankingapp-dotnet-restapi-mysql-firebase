using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.ResponseModels
{
    public class SignInResponse
    {
        public string Email { get; set; }

        public string IdToken { get; set; }
    }
}
