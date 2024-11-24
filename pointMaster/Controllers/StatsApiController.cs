using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pointMaster.Data;

namespace pointMaster.Controllers
{
    [ApiController]
    [Route("api")]
    public class StatsApiController : ControllerBase
    {
        readonly DataContext dataContext;

        public StatsApiController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [Authorize(Policy = Roles.Editor)]
        [HttpGet("GetPointsOverTime")]
        public async Task<object> GetList()
        {
            var pointData = await dataContext.Points.Include(x => x.Patrulje).Include(x => x.Poster).ToListAsync();

            var pointGrouped = pointData.OrderBy(x => x.DateCreated).GroupBy(x => x.Patrulje.Name).ToList();

            var retval = new List<PointChartModel>();

            var lastChange = DateTime.Now.ToString("MM/d/yyyy H:m:s").Replace('.', ':');

            foreach (var group in pointGrouped)
            {
                var data = new List<PointChartDataModel>();

                var total = 0;
                foreach (var point in group)
                {
                    var date = DateTime.Parse(point.DateCreated.ToString());

                    total += point.Points + point.Turnout;

                    data.Add(new PointChartDataModel
                    {
                        x = date.ToString("MM/d/yyyy H:m:s").Replace('.', ':'),
                        y = total
                    });
                }

                data.Add(new PointChartDataModel
                {
                    x = lastChange,
                    y = total
                });

                retval.Add(new PointChartModel
                {
                    Name = group.Key,
                    Data = data
                });
            }

            var rval = JsonConvert.SerializeObject(retval, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,

            });

            return Ok(rval);
        }

        [Route("getstats")]
        public async Task<IActionResult> GetStats()
        {
            var vm = new List<StatModel>();

            var pointData = await dataContext.Points.ToListAsync();
            var patruljeData = await dataContext.Patruljer.ToListAsync();
            var postData = await dataContext.Poster.ToListAsync();

            vm.Add(new StatModel
            {
                Title = "Points givet",
                Value = pointData.Sum(x => x.Points + x.Turnout).ToString()
            });

            vm.Add(new StatModel
            {
                Title = "Antal patruljer",
                Value = patruljeData.Count().ToString()
            });

            vm.Add(new StatModel
            {
                Title = "Antal poster",
                Value = postData.Count().ToString()
            });

            return Ok(JsonConvert.SerializeObject(vm));
        }

        public class PointChartModel
        {
            [JsonProperty("name")]
            public string Name { get; set; } = null!;
            [JsonProperty("data")]
            public List<PointChartDataModel> Data { get; set; } = null!;
        }
        public class PointChartDataModel
        {
            public string x { get; set; } = null!;
            public int y { get; set; }
        }

        public class StatModel
        {
            public string Title { get; set; } = null!;
            public string Value { get; set; } = null!;
        }
    }
}