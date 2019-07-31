using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MG_IdentityProvider.Entities
{
    public class MGUserContext : DbContext
    {
        public MGUserContext(DbContextOptions<MGUserContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
