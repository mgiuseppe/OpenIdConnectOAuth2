using MG_IdentityProvider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MG_IdentityProvider.Services
{
    public interface IMGUserRepository
    {
        User GetUserByUsername(string username);
        User GetUserBySubjectId(string subjectId);
        User GetUserByEmail(string email);
        User GetUserByProvider(string loginProvider, string providerKey);
        List<UserLogin> GetUserLoginsBySubjectId(string subjectId);
        List<UserClaim> GetUserClaimsBySubjectId(string subjectId);
        bool AreUserCredentialsValid(string username, string password);
        bool IsUserActive(string subjectId);
        void AddUser(User user);
        void AddUserLogin(string subjectId, string loginProvider, string providerKey);
        void AddUserClaim(string subjectId, string claimType, string claimValue);
        bool Save();
    }
}
