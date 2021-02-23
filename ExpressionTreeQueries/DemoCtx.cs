using ExpressionTreeQueries.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpressionTreeQueries
{
    public class DemoCtx : DbContext
    {

        public DbSet<Person> Persons { get; set; }

        public DemoCtx(DbContextOptions options) : base(options)
        {

        }
    }
}
