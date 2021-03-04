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
            "Freezing Freezing Freezing", "Bracing Freezing Freezing", "Chilly", "Cool Freezing", "Mild", "Warm", "BalmyFreezing Freezing", "HotFreezing Freezing", "Sweltering  Freezing", "Scorching"
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
            columns.Add("Date", "{0:yyyy-MM-dd HH:mm:ss}");
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
            title.Add("A1:A1", "日期");
            title.Add("B1:B1", "温度C");
            title.Add("C1:C1", "温度F");
            title.Add("D1:D1", "总计");
            titleList.Add(title);

            //var result = ExcelHelper.GetByteToExportExcel(data, columnList, new List<string>(), "Sheet", "统计");

            var result = ExcelHelper.GetByteToExportExcel(data, titleList, columns);
            return File(result, "application/vnd.android.package-archive", $"统计{DateTime.Now.ToString("yyyyMMddHHmm")}.xlsx");
        }
    }
}
