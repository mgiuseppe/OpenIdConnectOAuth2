using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MG_IdentityProvider.Entities;
using Microsoft.AspNetCore.Identity;

namespace MG_IdentityProvider.Services
{
    public class MGUserRepository : IMGUserRepository
    {
        private readonly MGUserContext _ctx;
        private readonly IPasswordHasher _passwordHasher;

        public MGUserRepository(MGUserContext ctx, IPasswordHasher passwordHasher)
        {
            _ctx = ctx;
            _passwordHasher = passwordHasher;
        }

        public void AddUser(User user)
        {
            //salt and hash the password
            if (!string.IsNullOrEmpty(user.Password))
                user.Password = _passwordHasher.HashPassword(user.Password);

            _ctx.Users.Add(user);
        }

        public void AddUserClaim(string subjectId, string claimType, string claimValue)
        {
            var user = GetUserBySubjectId(subjectId);
            if (user == null)
                throw new ArgumentException("User with given subjectId not found.", subjectId);

            user.Claims.Add(new UserClaim(claimType, claimValue));
        }

        public void AddUserLogin(string subjectId, string loginProvider, string providerKey)
        {
            var user = GetUserBySubjectId(subjectId);
            if (user == null)
                throw new ArgumentException("User with given subjectId not found.", subjectId);

            user.Logins.Add(new UserLogin()
            {
                SubjectId = subjectId,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
        }

        public bool AreUserCredentialsValid(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user == null || string.IsNullOrWhiteSpace(password))
                return false;

            //compare salted password
            return _passwordHasher.ValidatePassword(password, user.Password);
        }

        public User GetUserByEmail(string email)
            => _ctx.Users.FirstOrDefault(u => u.Claims.Any(c => c.ClaimType == "email" && c.ClaimValue == email));

        public User GetUserByProvider(string loginProvider, string providerKey) 
            => _ctx.Users.FirstOrDefault(u => u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey));

        public User GetUserBySubjectId(string subjectId) 
            => _ctx.Users.FirstOrDefault(u => u.SubjectId == subjectId);

        public User GetUserByUsername(string username) 
            => _ctx.Users.FirstOrDefault(u => u.Username == username);

        public List<UserClaim> GetUserClaimsBySubjectId(string subjectId) 
            => (_ctx.Users.FirstOrDefault(u => u.SubjectId == subjectId)?.Claims ?? new List<UserClaim>()).ToList();

        public List<UserLogin> GetUserLoginsBySubjectId(string subjectId) 
            => (_ctx.Users.FirstOrDefault(u => u.SubjectId == subjectId)?.Logins ?? new List<UserLogin>()).ToList();

        public bool IsUserActive(string subjectId) 
            => _ctx.Users.FirstOrDefault(u => u.SubjectId == subjectId).IsActive;

        public bool Save() 
            => _ctx.SaveChanges() > 0;
    }
}
