﻿using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// <param name="datas">数据列表</param>
        /// <param name="titleList">标题键值对，如果多单元格合并可用("A1:A2", "日期")</param>
        /// <param name="columnNames">指定导出数据列</param>
        /// <param name="sheetName">表名</param>
        /// <param name="isProtected">是否加密码</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static Byte[] GetByteToExportExcel<T>(List<T> datas, List<Dictionary<string, string>> titleList, List<string> columnNames = null, string sheetName = "Sheet", bool isProtected = false, string password = "")
        {
            using (var fs = new MemoryStream())
            {
                using (var package = CreateExcelPackage(datas, titleList, columnNames, sheetName, isProtected, password))
                {
                    package.SaveAs(fs);
                    return fs.ToArray();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="titleList"></param>
        /// <param name="columnNames"></param>
        /// <param name="sheetName"></param>
        /// <param name="isProtected"></param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        static ExcelPackage CreateExcelPackage<T>(List<T> datas, List<Dictionary<string, string>> titleList, List<string> columnNames, string sheetName, bool isProtected, string password)
        {
            if (datas == null)
            {
                datas = new List<T>();
            }
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var package = new ExcelPackage();
            var pageIndex = 0;
            var pageSize = 10000;
            while (pageIndex * pageSize <= datas.Count)
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName + pageIndex);
                CreateExcelSheet(datas.Skip(pageIndex * pageSize).Take(pageSize).ToList(), worksheet, titleList, columnNames, isProtected, password);
                pageIndex++;
            }
            return package;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="worksheet"></param>
        /// <param name="titleList"></param>
        /// <param name="columnNames"></param>
        /// <param name="isProtected"></param>
        /// <param name="password">密码</param>
        static void CreateExcelSheet<T>(List<T> datas, ExcelWorksheet worksheet, List<Dictionary<string, string>> titleList, List<string> columnNames, bool isProtected, string password)
        {
            if (isProtected && !string.IsNullOrEmpty(password))
            {
                worksheet.Protection.IsProtected = true;//设置是否进行锁定
                worksheet.Protection.SetPassword(password);//设置密码
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

            worksheet.Cells.Style.WrapText = true;
            worksheet.Cells.Style.ShrinkToFit = true;//单元格自动适应大小

            foreach (Dictionary<string, string> list in titleList)
            {
                foreach (var t in list)
                {
                    worksheet.Cells[t.Key].Merge = true;//合并单元格
                    worksheet.Cells[t.Key].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);//设置单元格所有边框
                    worksheet.Cells[t.Key].Value = t.Value;
                    worksheet.Cells[t.Key].Style.Font.Bold = true;
                    worksheet.Cells[t.Key].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                    worksheet.Cells[t.Key].Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                }
            }

            if (columnNames == null || columnNames.Count <= 0)
            {
                columnNames = typeof(T).GetProperties().Select(x => x.Name).ToList();
            }

            int row = 1 + titleList.Count;
            foreach (T data in datas)
            {
                int column = 1;
                foreach (string name in columnNames)
                {
                    var p = data.GetType().GetProperty(name);
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
                    worksheet.Cells[row, column].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);//设置单元格所有边框
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
