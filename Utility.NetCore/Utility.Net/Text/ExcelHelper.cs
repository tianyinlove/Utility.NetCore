using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Utility.Extensions;

namespace Utility.Text
{
    /// <summary>
    /// Excel导出
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="columnNames"></param>
        /// <param name="outOfColumn"></param>
        /// <param name="sheetName"></param>
        /// <param name="title"></param>
        /// <param name="isProtected"></param>
        /// <returns></returns>
        public static Byte[] GetByteToExportExcel<T>(List<T> datas, Dictionary<string, string> columnNames, List<string> outOfColumn, string sheetName = "Sheet", string title = "", bool isProtected = false)
        {
            using (var fs = new MemoryStream())
            {
                using (var package = CreateExcelPackage(datas, columnNames, outOfColumn, sheetName, title, isProtected))
                {
                    package.SaveAs(fs);
                    return fs.ToArray();
                }
            }
        }

        /// <summary>
        /// 创建ExcelPackage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas">数据实体</param>
        /// <param name="columnNames">列名</param>
        /// <param name="outOfColumns">排除列</param>
        /// <param name="sheetName">sheet名称</param>
        /// <param name="title">标题</param>
        /// <param name="isProtected">是否加密</param>
        /// <returns></returns>
        static ExcelPackage CreateExcelPackage<T>(List<T> datas, Dictionary<string, string> columnNames, List<string> outOfColumns, string sheetName, string title, bool isProtected)
        {
            if (datas == null)
            {
                datas = new List<T>();
            }
            var package = new ExcelPackage();
            var pageIndex = 0;
            var pageSize = 10000;
            while (pageIndex * pageSize <= datas.Count)
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName + pageIndex);
                CreateExcelSheet(datas.Skip(pageIndex * pageSize).Take(pageSize).ToList(), columnNames, outOfColumns, title, isProtected, worksheet);
                pageIndex++;
            }
            return package;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="columnNames"></param>
        /// <param name="outOfColumns"></param>
        /// <param name="title"></param>
        /// <param name="isProtected">设置是否进行锁定</param>
        /// <param name="worksheet"></param>
        static void CreateExcelSheet<T>(List<T> datas, Dictionary<string, string> columnNames, List<string> outOfColumns, string title, bool isProtected, ExcelWorksheet worksheet)
        {
            if (isProtected)
            {
                worksheet.Protection.IsProtected = true;//设置是否进行锁定
                worksheet.Protection.SetPassword("alex");//设置密码
                worksheet.Protection.AllowAutoFilter = false;//下面是一些锁定时权限的设置
                worksheet.Protection.AllowDeleteColumns = false;
                worksheet.Protection.AllowDeleteRows = false;
                worksheet.Protection.AllowEditScenarios = false;
                worksheet.Protection.AllowEditObject = false;
                worksheet.Protection.AllowFormatCells = false;
                worksheet.Protection.AllowFormatColumns = false;
                worksheet.Protection.AllowFormatRows = false;
                worksheet.Protection.AllowInsertColumns = false;
                worksheet.Protection.AllowInsertHyperlinks = false;
                worksheet.Protection.AllowInsertRows = false;
                worksheet.Protection.AllowPivotTables = false;
                worksheet.Protection.AllowSelectLockedCells = false;
                worksheet.Protection.AllowSelectUnlockedCells = false;
                worksheet.Protection.AllowSort = false;
            }

            var titleRow = 0;
            if (!string.IsNullOrWhiteSpace(title))
            {
                titleRow = 1;
                worksheet.Cells[1, 1, 1, columnNames.Count()].Merge = true;//合并单元格
                worksheet.Cells[1, 1].Value = title;
                worksheet.Cells.Style.WrapText = true;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                worksheet.Row(1).Height = 30;//设置行高
                worksheet.Cells.Style.ShrinkToFit = true;//单元格自动适应大小
            }

            //获取要反射的属性,加载首行
            Type myType = typeof(T);
            List<PropertyInfo> myPro = new List<PropertyInfo>();
            int i = 1;
            foreach (string key in columnNames.Keys)
            {
                PropertyInfo p = myType.GetProperty(key);
                myPro.Add(p);
                worksheet.Cells[1 + titleRow, i].Value = columnNames[key];
                worksheet.Cells[1 + titleRow, i].Style.Font.Bold = true;
                worksheet.Cells[1 + titleRow, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                worksheet.Cells[1 + titleRow, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                i++;
            }

            int row = 2 + titleRow;
            foreach (T data in datas)
            {
                int column = 1;
                foreach (PropertyInfo p in myPro.Where(info => !outOfColumns.Contains(info.Name)))
                {
                    if (string.Equals(p.PropertyType.BaseType.Name, "Enum", StringComparison.OrdinalIgnoreCase))
                    {
                        worksheet.Cells[row, column].Value = p == null ? "" : GetDisplayName((p.GetValue(data, null) as Enum));
                    }
                    else if (string.Equals(p.PropertyType.Name, "decimal", StringComparison.OrdinalIgnoreCase))
                    {
                        worksheet.Cells[row, column].Value = p == null ? "" : string.Format("{0:F2}", p.GetValue(data, null));
                    }
                    else
                    {
                        worksheet.Cells[row, column].Value = p == null ? "" : Convert.ToString(p.GetValue(data, null));
                    }
                    column++;
                }
                row++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        static string GetDisplayName(Enum states)
        {
            return states.GetDescription();
        }
    }
}
