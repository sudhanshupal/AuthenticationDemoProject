using AuthenticationAPIDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPIDemo.Data
{
    public class MyDBContext:DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options):base(options) { } 
        public DbSet<User> Users { get; set; }

    }
}
