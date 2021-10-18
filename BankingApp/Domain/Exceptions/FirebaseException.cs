using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class FirebaseException : Exception
    {
        public override string Message { get; }

        public int StatusCode { get; }

        public FirebaseException(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
