using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MG_IdentityProvider.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool ValidatePassword(string password, string savedPasswordSaltAndHash);
    }
}
