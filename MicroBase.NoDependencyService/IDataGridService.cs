using MicroBase.Share;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using MicroBase.Share.Extensions;
using MicroBase.Share.Constants;
using MicroBase.Share.Models.DataGirds;

namespace MicroBase.NoDependencyService
{
    public interface IDataGridService
    {
        IEnumerable<T> GetDataFromExcelFile<T>(IFormFile formFile, IEnumerable<ExcelFieldMapping> fieldMappings);

        IEnumerable<T> GetRawDataFromExcelFile<T>(IFormFile formFile, Dictionary<string, string> fields);

        byte[] ExportToCsv<T>(IEnumerable<T> source);

        byte[] ExportToCsv<T>(Dictionary<string, string> headers, IEnumerable<T> source);
    }

    public class DataGridService : IDataGridService
    {
        public IEnumerable<T> GetDataFromExcelFile<T>(IFormFile formFile, IEnumerable<ExcelFieldMapping> fieldMappings)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var results = new List<T>();
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    ms.Position = 0;

                    using (ExcelPackage package = new ExcelPackage(ms))
                    {
                        var workbook = package.Workbook;
                        var worksheet = workbook.Worksheets[0];
                        var colsTitle = new List<string>();
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            var title = worksheet.Cells[1, col].Value == null ? string.Empty : worksheet.Cells[1, col].Value.ToString();
                            colsTitle.Add(title);
                        }

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var entityModel = (T)Activator.CreateInstance(typeof(T));
                            for (int i = 1; i <= colsTitle.Count; i++)
                            {
                                var value = worksheet.Cells[row, i].Value == null
                                    ? string.Empty
                                    : worksheet.Cells[row, i].Value.ToString();

                                var fieldMapping = fieldMappings.FirstOrDefault(s => s.ExcelColumn == colsTitle[i - 1]);
                                if (fieldMapping == null)
                                {
                                    continue;
                                }

                                var field = entityModel.GetType().GetProperty(fieldMapping.EntityColumn);
                                if (field == null)
                                {
                                    continue;
                                }

                                if (field.PropertyType == typeof(DateTime) || field.PropertyType == typeof(DateTime?))
                                {
                                    DateTime date = Convert.ToDateTime(value);
                                    field.SetValue(entityModel, date);
                                }
                                else if (field.PropertyType == typeof(int) || field.PropertyType == typeof(int?))
                                {
                                    var isSuccess = int.TryParse(value, out var val);
                                    if (isSuccess)
                                    {
                                        field.SetValue(entityModel, val);
                                    }
                                }
                                else if (field.PropertyType == typeof(float) || field.PropertyType == typeof(float?))
                                {
                                    var isSuccess = float.TryParse(value, out var val);
                                    if (isSuccess)
                                    {
                                        field.SetValue(entityModel, val);
                                    }
                                }
                                else if (field.PropertyType == typeof(decimal) || field.PropertyType == typeof(decimal?))
                                {
                                    var isSuccess = decimal.TryParse(value, out var val);
                                    if (isSuccess)
                                    {
                                        field.SetValue(entityModel, val);
                                    }
                                }
                                else if (field.PropertyType == typeof(bool) || field.PropertyType == typeof(bool?))
                                {
                                    var isSuccess = bool.TryParse(value, out var val);
                                    if (isSuccess)
                                    {
                                        field.SetValue(entityModel, val);
                                    }
                                }
                                else
                                {
                                    entityModel.GetType().GetProperty(fieldMapping.EntityColumn).SetValue(entityModel, value);
                                }
                            }

                            results.Add(entityModel);
                        }

                        package.Dispose();
                    }

                    ms.Dispose();
                    ms.Close();
                }

                return results;
            }
            catch (Exception ex)
            {
                return new List<T>();
            }
        }

        public IEnumerable<T> GetRawDataFromExcelFile<T>(IFormFile formFile, Dictionary<string, string> fields)
        {
            try
            {
                var results = new List<T>();
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    ms.Position = 0;

                    using (ExcelPackage package = new ExcelPackage(ms))
                    {
                        var workbook = package.Workbook;
                        var worksheet = workbook.Worksheets[1];
                        var colsTitle = new List<string>();
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            var title = worksheet.Cells[1, col].Value == null ? string.Empty : worksheet.Cells[1, col].Value.ToString();
                            colsTitle.Add(title);
                        }

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var product = (T)Activator.CreateInstance(typeof(T));
                            for (int i = 1; i <= colsTitle.Count; i++)
                            {
                                var value = worksheet.Cells[row, i].Text == null ? string.Empty : worksheet.Cells[row, i].Text;
                                if (fields.ContainsKey(colsTitle[i - 1]))
                                {
                                    var fieldName = fields[colsTitle[i - 1]];
                                    var field = product.GetType().GetProperty(fieldName);
                                    if (field == null)
                                    {
                                        continue;
                                    }

                                    product.GetType().GetProperty(fieldName).SetValue(product, value);
                                }
                            }

                            results.Add(product);
                        }

                        package.Dispose();
                    }

                    ms.Dispose();
                    ms.Close();
                }

                return results;
            }
            catch (Exception ex)
            {
                return new List<T>();
            }
        }

        public byte[] ExportToCsv<T>(IEnumerable<T> source)
        {
            var headers = new List<string> { };
            var displayHeaders = new List<string> { };

            var properties = typeof(T).GetProperties();
            foreach (var pro in properties)
            {
                var displayName = pro.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                if (displayName != null)
                {
                    if (displayName.Name.Equals(Constants.ExportCSV.HIDE_COLUMN))
                    {
                        continue;
                    }
                    else
                    {
                        displayHeaders.Add(displayName.Name);
                    }
                }
                else
                {
                    displayHeaders.Add(pro.Name);
                }

                headers.Add(pro.Name);
            }

            var csv = new StringBuilder();
            csv.AppendLine(displayHeaders.Join(","));

            foreach (var item in source)
            {
                var arrs = new List<string>();
                foreach (var header in headers)
                {
                    var pro = properties.FirstOrDefault(s => s.Name == header);
                    if (pro == null)
                    {
                        continue;
                    }

                    var val = pro.GetValue(item);
                    arrs.Add(val != null ? val.ToString() : string.Empty);
                }

                csv.AppendLine(arrs.Join(","));
            }

            var data = Encoding.UTF8.GetBytes(csv.ToString());
            var result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();
            return result;
        }

        public byte[] ExportToCsv<T>(Dictionary<string, string> headers, IEnumerable<T> source)
        {
            var headerFields = new List<string> { };

            var properties = typeof(T).GetProperties();
            foreach (var header in headers)
            {
                headerFields.Add(header.Value);
            }

            var csv = new StringBuilder();
            csv.AppendLine(headerFields.Join(","));

            foreach (var item in source)
            {
                var arrs = new List<string>();
                foreach (var header in headers)
                {
                    var pro = properties.FirstOrDefault(s => s.Name == header.Key);
                    if (pro == null)
                    {
                        arrs.Add(String.Empty);
                        continue;
                    }

                    string fieldValue = string.Empty;

                    var objVal = pro.GetValue(item);
                    if (objVal == null)
                    {
                        arrs.Add(String.Empty);
                        continue;
                    }

                    if (pro.PropertyType == typeof(DateTime) || pro.PropertyType == typeof(DateTime?))
                    {
                        fieldValue = DateTime.Parse(objVal.ToString()).ToString(StaticModel.DATE_TIME_FORMAT);
                    }
                    else
                    {
                        fieldValue = objVal.ToString().Replace('\n', ' ').Replace('\r', ' ').Replace(",", " ");
                    }

                    arrs.Add(fieldValue);
                }

                csv.AppendLine(arrs.Join(","));
            }

            var data = Encoding.UTF8.GetBytes(csv.ToString());
            var result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();
            return result;
        }
    }
}