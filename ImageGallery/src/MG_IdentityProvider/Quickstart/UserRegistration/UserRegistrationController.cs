using IdentityServer4.Services;
using MG_IdentityProvider.Entities;
using MG_IdentityProvider.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MG_IdentityProvider.Quickstart.UserRegistration
{
    public class UserRegistrationController : Controller
    {
        private readonly IMGUserRepository _mgUserRepository;
        private readonly IIdentityServerInteractionService _interaction;

        public UserRegistrationController(IMGUserRepository mgUserRepository, IIdentityServerInteractionService interaction)
        {
            _mgUserRepository = mgUserRepository;
            _interaction = interaction;
        }

        public IActionResult RegisterUser(RegistrationInputModel registrationInputModel)
        {
            var vm = new RegisterUserViewModel()
            {
                ReturnUrl = registrationInputModel.ReturnUrl,
                Provider = registrationInputModel.Provider,
                ProviderUserId = registrationInputModel.ProviderUserId,
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                //create user + claims
                var userToCreate = new User()
                {
                    Username = model.UserName,
                    Password = model.Password,
                    IsActive = true,
                    Claims =
                    {
                        new UserClaim("country", model.Country),
                        new UserClaim("address", model.Address),
                        new UserClaim("given_name", model.FirstName),
                        new UserClaim("family_name", model.LastName),
                        new UserClaim("email", model.Email),
                        new UserClaim("subscriptionlevel", "FreeUser"),
                    },
                };

                // if we're provisioning user from external login we've to add provider and provideruserid to the user's logins
                if(model.IsProvisioningFromExternal)
                    userToCreate.Logins.Add(new UserLogin { LoginProvider = model.Provider, ProviderKey = model.ProviderUserId });

                //add it through the repository
                _mgUserRepository.AddUser(userToCreate);

                if (!_mgUserRepository.Save())
                    throw new Exception("Creating a user failed.");

                //log the user in
                if(!model.IsProvisioningFromExternal)
                    await HttpContext.SignInAsync(userToCreate.SubjectId, userToCreate.Username);

                //continue with the flow
                if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return Redirect("~/");
            } 
            
            return View(model);
        }
    }
}
