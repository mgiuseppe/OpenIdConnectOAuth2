using MG_IdentityProvider.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MG_IdentityProvider.Entities
{
    public static class MGUserContextExtensions
    {
        public static void EnsureSeedDataForContext(this MGUserContext context, IPasswordHasher passwordHasher)
        {
            if (context.Users.Any())
                return;

            var users = new List<User>(){
                new User
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "Giuseppe",
                    Password = passwordHasher.HashPassword("password"),
                    IsActive = true,
                    Claims = new List<UserClaim>
                        {
                            new UserClaim("given_name", "Giuseppe"),
                            new UserClaim("family_name", "Underwood"),
                            new UserClaim("address", "tree road 52"),
                            new UserClaim("role", "FreeUser"),
                            new UserClaim("subscriptionLevel", "FreeUser"),
                            new UserClaim("country", "it")
                        }
                },
                new User
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "Giancarlo",
                    Password = passwordHasher.HashPassword("password"),
                    IsActive = true,
                    Claims = new List<UserClaim>
                    {
                        new UserClaim("given_name", "Giancarlo"),
                        new UserClaim("family_name", "Underwood"),
                        new UserClaim("address", "tree road 42"),
                        new UserClaim("role", "PayingUser"),
                        new UserClaim("subscriptionLevel", "PayingUser"),
                        new UserClaim("country", "be")
                    }
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
