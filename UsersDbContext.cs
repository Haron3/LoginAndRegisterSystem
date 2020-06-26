using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginAndRegisterSystem
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext() : base("UsersDbConnectionString")
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
