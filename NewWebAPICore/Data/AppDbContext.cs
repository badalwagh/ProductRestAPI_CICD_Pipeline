using Microsoft.EntityFrameworkCore;
using NewWebAPICore.Model;
using WebAPICore.Model;

namespace WebAPICore.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> op)
            :base(op)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Production> Productions { get; set; }
        public DbSet<ErrorLogs> ErrorLogs { get; set; }

    }
}
