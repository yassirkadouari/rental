using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace CarRentalApp.Services;

public class ExcelService
{
    public byte[] ExportToExcel<T>(List<T> data, string sheetName)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add(sheetName);
            
            // Create headers from properties
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
            }

            // Fill data
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    object val = properties[j].GetValue(data[i]);
                    worksheet.Cell(i + 2, j + 1).Value = val?.ToString() ?? "";
                }
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
    public List<CarRentalApp.Core.Entities.Vehicle> ImportVehicles(string filePath)
    {
        var list = new List<CarRentalApp.Core.Entities.Vehicle>();
        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed();
            
            // Skip header
            foreach (var row in rows.Skip(1))
            {
                try 
                {
                    // Minimal import logic assuming specific column order or simple implementation
                    // Brand, Model, Year, DailyRate, LicensePlate
                    var v = new CarRentalApp.Core.Entities.Vehicle
                    {
                        Brand = row.Cell(1).GetValue<string>(),
                        Model = row.Cell(2).GetValue<string>(),
                        Year = int.Parse(row.Cell(3).GetValue<string>()),
                        DailyRate = decimal.Parse(row.Cell(4).GetValue<string>()),
                        LicensePlate = row.Cell(5).GetValue<string>(),
                        Status = CarRentalApp.Core.Enums.VehicleStatus.Available,
                        VehicleTypeId = 1 // Default to 1, or try to find by name if complex import
                    };
                    list.Add(v);
                }
                catch
                {
                    // Skip invalid rows
                }
            }
        }
        return list;
    }
}
