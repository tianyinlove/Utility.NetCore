using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Utility.Http;
using Utility.Extensions;
using Utility.Constants;
using Utility.NetLog;
using Utility.Text;

namespace Utility.NetCore.TestWeb.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastController()
        {
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            Logger.WriteLog(LogLevel.Info, "测试");
            var rng = new Random();
            var list = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
            return list;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExportExcel()
        {
            var data = Get().ToList();
            var columns = new Dictionary<string, string>();
            columns.Add("Date", "{0:yyyy-MM}");
            columns.Add("TemperatureC", "{0:F2}");
            columns.Add("TemperatureF", "");
            columns.Add("Summary", "");

            //var titleList = new List<List<ExcelTitle>>();
            //titleList.Add(new List<ExcelTitle>
            //{
            //    new ExcelTitle { Name = "日期",FromRow=1,FromCol=1,ToRow = 2, ToCol = 1 },
            //    new ExcelTitle { Name = "温度",FromRow=1,FromCol=2,ToRow = 1, ToCol = 3 },
            //    new ExcelTitle { Name = "总计",FromRow=1,FromCol=4,ToRow = 2, ToCol = 4 }
            //});

            //titleList.Add(new List<ExcelTitle>
            //{
            //    new ExcelTitle { Name = "C",FromRow=2,FromCol=2 },
            //    new ExcelTitle { Name = "F",FromRow=2,FromCol=3}
            //});

            var titleList = new List<Dictionary<string, string>>();
            var title = new Dictionary<string, string>();
            title.Add("A1:D1", "统计");
            titleList.Add(title);

            title = new Dictionary<string, string>();
            title.Add("A3:A4", "日期");
            title.Add("B3:C3", "温度");
            title.Add("D3:D4", "总计");
            titleList.Add(title);

            title = new Dictionary<string, string>();
            title.Add("B4", "C");
            title.Add("C4", "F");
            titleList.Add(title);

            //var result = ExcelHelper.GetByteToExportExcel(data, columnList, new List<string>(), "Sheet", "统计");

            var result = ExcelHelper.GetByteToExportExcel(data, titleList, columns, isProtected: true, password: "123");
            return File(result, "application/vnd.android.package-archive", $"统计{DateTime.Now.ToString("yyyyMMddHHmm")}.xlsx");
        }
    }
}
