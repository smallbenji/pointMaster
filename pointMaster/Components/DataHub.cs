using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using pointMaster.Controllers;
using pointMaster.Data;

namespace pointMaster.Components
{
    [Authorize(Policy = Roles.Editor)]
    public class DataHub : Hub
    {
        private readonly DataContext dataContext;

        public DataHub(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("ReceiveMessage", "Hey");
            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendData()
        {
            var v0 = await StatModel.Calculate(dataContext);
            var v1 = await PointRatioModel.Calculate(dataContext);
            var v2 = await PointChartModel.Calculate(dataContext);

            var retval = new StatData()
            {
                Stats = v0,
                PointRatio = v1,
                PointChartModels = v2,
            };
            await Clients.All.SendAsync("StatData", retval);
        }

        public class StatData
        {
            public List<StatModel> Stats { get; set; }
            public PointRatioModel PointRatio { get; set; }
            public List<PointChartModel> PointChartModels { get; set; }
        }
    }
}
