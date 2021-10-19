using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Models.ReadModels
{
    public class UserReadModel
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string LocalId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
