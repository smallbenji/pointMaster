using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pointMaster.Data;

namespace pointMaster.Controllers
{
    [Authorize(Policy = Roles.Editor)]
    [ApiController]
    [Route("api")]
    public class StatsApiController : ControllerBase
    {
        readonly DataContext dataContext;

        public StatsApiController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet("GetPointsOverTime")]
        public async Task<object> GetList()
        {
            var retval = await PointChartModel.Calculate(dataContext);

            var rval = JsonConvert.SerializeObject(retval, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,

            });

            return Ok(rval);
        }

        [Route("getstats")]
        public async Task<IActionResult> GetStats()
        {
            var vm = await StatModel.Calculate(dataContext);

            return Ok(JsonConvert.SerializeObject(vm));
        }

        [Route("pointratio")]
        public async Task<IActionResult> GetPointRatio()
        {
            var vm = await PointRatioModel.Calculate(dataContext);

            return Ok(JsonConvert.SerializeObject(vm));
        }
    }
    public class PointChartModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("data")]
        public List<PointChartDataModel> Data { get; set; }

        public static async Task<List<PointChartModel>> Calculate(DataContext dataContext)
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
                    var date = DateTime.Parse(point.DateCreated.ToString()).AddHours(1);

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

            return retval;
        }
    }
    public class PointChartDataModel
    {
        public string x { get; set; }
        public int y { get; set; }
    }

    public class StatModel
    {
        public string Title { get; set; }
        public string Value { get; set; }

        public static async Task<List<StatModel>> Calculate(DataContext dataContext)
        {
            var vm = new List<StatModel>();

            var pointData = await dataContext.Points.ToListAsync();
            var patruljeData = await dataContext.Patruljer.ToListAsync();
            var postData = await dataContext.Poster.ToListAsync();
            var medlemsData = await dataContext.PatruljeMedlemmer.ToListAsync();

            vm.Add(new StatModel
            {
                Title = "Points givet ialt",
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

            vm.Add(new StatModel
            {
                Title = "Transaktioner",
                Value = pointData.Count().ToString()
            });

            vm.Add(new StatModel
            {
                Title = "Medlemmer",
                Value = medlemsData.Count().ToString()
            });

            return vm;
        }
    }

    public class PointRatioModel
    {
        [JsonProperty("names")]
        public List<string> Names { get; set; }
        [JsonProperty("data")]
        public List<int> Data { get; set; }

        public static async Task<PointRatioModel> Calculate(DataContext dataContext)
        {
            var vm = new PointRatioModel
            {
                Names = new List<string>(),
                Data = new List<int>()
            };

            var data = await dataContext.Patruljer.Include(x => x.Points).ToListAsync();

            foreach (var patrulje in data)
            {
                vm.Names.Add(patrulje.Name);
                vm.Data.Add(patrulje.Points.Sum(x => x.Points + x.Turnout));
            }

            return vm;
        }
    }

}