using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using pointMaster.Components;
using pointMaster.Controllers;
using pointMaster.Models;

namespace pointMaster.Data
{
    public class DataContext : DbContext
    {
        private readonly IHubContext<DataHub> _hubContext;
        public DataContext(DbContextOptions<DataContext> options, IHubContext<DataHub> hubContext) : base(options)
        {
            _hubContext = hubContext;
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(CancellationToken.None);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            var v0 = await StatModel.Calculate(this);
            var v1 = await PointRatioModel.Calculate(this);
            var v2 = await PointChartModel.Calculate(this);

            await _hubContext.Clients.All.SendAsync("StatData", new DataHub.StatData
            {
                Stats = v0,
                PointRatio = v1,
                PointChartModels = v2
            });
            return result;
        }

        public DbSet<Patrulje> Patruljer { get; set; }
        public DbSet<PatruljeMedlem> PatruljeMedlemmer { get; set; }
        public DbSet<Post> Poster { get; set; }
        public DbSet<Point> Points { get; set; }
    }
}
