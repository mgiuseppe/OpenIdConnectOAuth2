using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MG_IdentityProvider.Quickstart.UserRegistration
{
    public class RegisterUserViewModel
    {
        //Credentials
        [MaxLength(100)]
        public string UserName { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        //Claims
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [MaxLength(2)]
        public string Country { get; set; }

        public SelectList CountryCodes { get; set; } = 
            new SelectList(
                new[] {
                    new {Id = "IT", Value="Italy"},
                    new {Id = "BE", Value="Belgium"}
                }, 
                "Id", 
                "Value"
            );

        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
        public bool IsProvisioningFromExternal => Provider != null;

        public string ReturnUrl { get; set; }
    }
}
