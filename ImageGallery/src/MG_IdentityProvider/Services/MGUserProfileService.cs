using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MG_IdentityProvider.Services
{
    public class MGUserProfileService : IProfileService
    {
        private readonly IMGUserRepository _mgUserRepository;

        public MGUserProfileService(IMGUserRepository mgUserRepository)
        {
            _mgUserRepository = mgUserRepository;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var claimsForUser = _mgUserRepository.GetUserClaimsBySubjectId(subjectId);

            context.IssuedClaims = claimsForUser.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            context.IsActive = _mgUserRepository.GetUserBySubjectId(subjectId).IsActive;

            return Task.FromResult(0);
        }
    }
}
