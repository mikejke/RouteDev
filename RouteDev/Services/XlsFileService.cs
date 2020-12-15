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

                    for(var i = 0; i < transportList.Count; i++)
                    {
                        var row = i + 2;
                        ws.Cell(row, 1).Value = 
                            i + 1;
                        ws.Cell(row, 2).Value = 
                            transportList[i].Type.ToString("F");
                        ws.Cell(row, 3).Value =
                            transportList[i].Capacity.ToString("## 'кг'");
                        ws.Cell(row, 4).Value =
                            transportList[i].Distance.ToString("## 'км'");
                        ws.Cell(row, 5).Value =
                            transportList[i].TotalUploadingTime.ToString("##.## 'часов'");
                        ws.Cell(row, 6).Value =
                            transportList[i].WorkingHours.ToString("##.## 'часов'");
                        ws.Cell(row, 7).Value =
                            string.Join(" - ", transportList[i].Route.Select(s => s.Id));
                        ws.Cell(row, 8).Value =
                            transportList[i].Expenses.ToString("## 'у.е'");
                    }

                    //итого
                    var conclusion = transportList.Count() + 2;
                    ws.Cell(conclusion, 1).Value =
                        "Итого";
                    ws.Cell(conclusion, 3).Value =
                        transportList.Sum(c => c.Capacity).ToString("## 'кг'");
                    ws.Cell(conclusion, 4).Value =
                        transportList.Sum(c => c.Distance).ToString("## 'км'");
                    ws.Cell(conclusion, 5).Value =
                        transportList.Sum(c => c.TotalUploadingTime).ToString("##.## 'часов'");
                    ws.Cell(conclusion, 6).Value =
                        transportList.Sum(c => c.WorkingHours).ToString("##.## 'часов'");
                    ws.Cell(conclusion, 8).Value =
                        transportList.Sum(c => c.Expenses).ToString("## 'у.е'");

                    //Штрафы
                    //если какой то магазин не получил товаров.
                    if (shopList.Any(shop => shop.AnyNeed()))
                    {
                        var sanctions = conclusion + 1;
                        ws.Cell(sanctions, 1).Value =
                            "Штрафы";
                        sanctions += 1;
                        ws.Cell(sanctions, 1).Value =
                            "Магазин";
                        ws.Cell(sanctions, 1).Value =
                            "Необходимые товары";
                        ws.Cell(sanctions, 1).Value =
                            "Сумма штрафа";
                        sanctions += 1;
                        foreach (var shop in shopList.Where(s => s.AnyNeed()))
                        {
                            ws.Cell(sanctions, 1).Value =
                                shop.Title;
                            ws.Cell(sanctions, 2).Value =
                                $"{shop.Products} - {shop.Chemistry} - {shop.Drinks}";
                            ws.Cell(sanctions, 3).Value =
                                shop.Sum * 3;
                        }

                        sanctions += 1;
                        var products = shopList.Where(shop => shop.AnyNeed()).Select(s => s.Products).Sum();
                        var chemistry = shopList.Where(shop => shop.AnyNeed()).Select(s => s.Chemistry).Sum();
                        var drinks = shopList.Where(shop => shop.AnyNeed()).Select(s => s.Drinks).Sum();
                        ws.Cell(sanctions, 1).Value =
                            "Итого";
                        ws.Cell(sanctions, 1).Value =
                            $"{products} - {chemistry} - {drinks}";
                        ws.Cell(sanctions, 1).Value =
                            shopList.Where(shop => shop.AnyNeed()).Select(s => s.Sum).Sum() * 3;
                    }


                    ws.Columns().AdjustToContents();
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