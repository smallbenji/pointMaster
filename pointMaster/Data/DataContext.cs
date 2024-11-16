using Microsoft.EntityFrameworkCore;
using pointMaster.Models;

namespace pointMaster.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Patrulje> Patruljer { get; set; } = default!;
        public DbSet<PatruljeMedlem> PatruljeMedlemmer { get; set; } = default!;
        public DbSet<Post> Poster { get; set; } = default!;
        public DbSet<Point> Points { get; set; } = default!;
    }
}
