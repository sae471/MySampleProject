
using Microsoft.EntityFrameworkCore;
using SAE471.Sample.Domain.Aggregates;
using SAE471.EFCore;

namespace SAE471.Sample.EFCore
{
    public class AppDbContext : SAE471DbContext
    {
        public DbSet<Customer> Customers { get; set; }
    }
}