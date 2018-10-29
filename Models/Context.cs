using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
 
namespace e_commerce.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public IHostingEnvironment he;
	    public DbSet<User> User { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Order> Order { get; set; }


    }
}
