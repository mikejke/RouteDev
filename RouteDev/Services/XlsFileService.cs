using ClosedXML.Excel;
using RouteDev.Data;
using RouteDev.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RouteDev.Services
{
    /// <summary>
    /// Service to import/export data in xls file format.
    /// </summary>
    public class XlsFileService : IFileService
    {
        private int row;

        public List<Shop> Open(string filename)
        {
            var shopList = new List<Shop>();

            using (var wb = new XLWorkbook(filename))
            {
                try
                {
                    foreach (var ws in wb.Worksheets)
                    {
                        foreach(var row in ws.Rows())
                        {
                            //var row = ws.Row(i);

                            shopList.Add(new Shop(
                                row.Cell(1).GetValue<uint>(),
                                row.Cell(2).GetValue<byte>(),
                                row.Cell(3).GetValue<byte>())
                            {
                                Products = int.Parse(row.Cell(4).GetValue<string>().Replace('-', '0')),
                                Chemistry = int.Parse(row.Cell(5).GetValue<string>().Replace('-', '0')),
                                Drinks = int.Parse(row.Cell(6).GetValue<string>().Replace('-', '0'))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return shopList;
        }

        public void Save(string filename, List<Shop> shopList, List<Transport> transportList)
        {
            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws =
                        wb.Worksheets.Add("Transport");
                    ws.Cell(1, 1).Value = "#";
                    ws.Cell(1, 2).Value = "Тип";
                    ws.Cell(1, 3).Value = "Вместимость";
                    ws.Cell(1, 4).Value = "Длинна маршрута";
                    ws.Cell(1, 5).Value = 
                        "Потрачено времени на разгрузку/загрузку";
                    ws.Cell(1, 6).Value = "Общее рабочее время машины";
                    ws.Cell(1, 7).Value = "Маршрут перевозки";
                    ws.Cell(1, 8).Value = "Затраты";

                    transportList.ForEach(car =>
                    {
                        var row = transportList.IndexOf(car) + 2;
                        ws.Cell(this.row, 1).Value = 
                            this.row;
                        ws.Cell(this.row, 2).Value = 
                            car.Type.ToString("F");
                        ws.Cell(this.row, 3).Value =
                            car.Capacity.ToString("## 'кг'");
                        ws.Cell(this.row, 4).Value =
                            car.Distance.ToString("## 'км'");
                        ws.Cell(this.row, 5).Value =
                            car.TotalUploadingTime.ToString("##.## 'часов'");
                        ws.Cell(this.row, 6).Value =
                            car.WorkingHours.ToString("## 'часов'");
                        ws.Cell(this.row, 7).Value =
                            string.Join(" - ", car.Route.Select(s => s.Id)).Replace("0", "Склад");
                        ws.Cell(this.row, 8).Value =
                            car.Expenses.ToString("## 'у.е'");
                    });

                    //итого
                    ws.Cell(transportList.Count() + 1, 1).Value =
                        "Итого";
                    ws.Cell(transportList.Count() + 1, 3).Value =
                        transportList.Sum(c => c.Capacity).ToString("## 'кг'");
                    ws.Cell(transportList.Count() + 1, 4).Value =
                        transportList.Sum(c => c.Distance).ToString("## 'км'");
                    ws.Cell(transportList.Count() + 1, 5).Value =
                        transportList.Sum(c => c.TotalUploadingTime).ToString("##.## 'часов'");
                    ws.Cell(transportList.Count() + 1, 6).Value =
                        transportList.Sum(c => c.WorkingHours).ToString("## 'часов'");
                    ws.Cell(transportList.Count() + 1, 8).Value =
                        transportList.Sum(c => c.Expenses).ToString("## 'у.е'");

                    //TODO: add a image of map to xls sheet

                    using (var fs = new FileStream(filename, FileMode.Create))
                    {
                        wb.SaveAs(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}