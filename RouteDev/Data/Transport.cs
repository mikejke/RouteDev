using System;
using System.Collections.Generic;
using System.Linq;

namespace RouteDev.Data
{
    public class Transport : Cargo
    {
        public double TotalUploadingTime { get; private set; } = 0;
        public TransportType Type { get; set; }
        public double FixedExpenses { get; private set; }
        public double VarExpenses { get; private set; } = 0.5;

        public short Capacity { get; private set; }

        public int Load => Sum;

        public List<Shop> Route { get; set; } = new List<Shop>() { new Shop(0, StorageX, StorageY) };

        public override int Products
        {
            get => _products;
            set
            {
                if (_chemistry + _drinks + value <= Capacity)
                {
                    _products = value;
                    OnPropertyChanged(nameof(Products));
                }
            }
        }
        public override int Chemistry
        {
            get => _chemistry;
            set
            {
                if (_products + _drinks + value <= Capacity)
                {
                    _chemistry = value;
                    OnPropertyChanged(nameof(Chemistry));
                }
            }
        }
        public override int Drinks
        {
            get => _drinks;
            set
            {
                if (_chemistry + _products + value <= Capacity)
                {
                    _drinks = value;
                    OnPropertyChanged(nameof(Drinks));
                }
            }
        }

        private const byte MaxWorkTime = 11;
        private const byte MinWorkTime = 6;
        private const byte AvgWorkTime = 8;
        private const double UploadingTime = 0.5; // MINUTES

        public Transport(TransportType type)
        {
            Type = type;
            switch (type)
            {
                case TransportType.Own:
                    FixedExpenses = 10;
                    Capacity = 120;
                    break;
                case TransportType.Hired:
                    FixedExpenses = 50;
                    Capacity = 150;
                    break;
            }
        }

        /// <summary>
        /// Расчет проработаного времени
        /// </summary>
        /// <returns></returns>
        public double WorkingHours
        {
            get
            {
                var time = TotalUploadingTime;
                if (time > 5.5 || Distance > 110)
                {
                    time += 0.5;
                }

                time += Distance * 3 / 60;

                //if (time > MaxWorkTime)
                //    throw new Exception("Больше максимума");

                return time;
            }
        }

        /// <summary>
        /// Расчет расходов машины
        /// </summary>
        /// <returns></returns>
        public double Expenses
        {
            get
            {
                double expenses = 0;
                expenses += VarExpenses * Distance;
                    //Переработка
                if (WorkingHours > AvgWorkTime)
                {
                    expenses += (WorkingHours - AvgWorkTime) * 15;
                }
                //Неполное использование транспорта по времени
                if (WorkingHours < MinWorkTime && Type == TransportType.Own)
                {
                    expenses += 10;
                }
                else if (WorkingHours < MinWorkTime && Type == TransportType.Hired)
                {
                    expenses += 15;
                }
                //Неполное использование вместимости транспортного средства
                if (Load < 90)
                {
                    expenses += 2 * (90 - Load);
                }

                return expenses;
            }
        }

        public bool Overworking => WorkingHours >= AvgWorkTime;
        

        public void Unload(int p, int c, int d)
        {
            Products -= p;
            Chemistry -= c;
            Drinks -= d;

            TotalUploadingTime += ((p + c + d) * UploadingTime) / 60; 
        }

        public void Upload()
        {
            Products = 40;
            Chemistry = 40;
            Drinks = 40;

            TotalUploadingTime += 0.5;
        }

        public short Distance 
        {
            get
            {
                short distance = 0;
                for (int i = 1; i < Route.Count; i++)
                {
                    distance +=
                        (short) (Math.Abs(Route[i - 1].X - Route[i].X) + (Math.Abs(Route[i - 1].Y - Route[i].Y)));
                }
                return distance;
            }
        }

        public bool WillOverwork(double distance)
        {
            return (distance * 3 / 60) + WorkingHours >= AvgWorkTime;
        }

        public void CalculateRoutes(IEnumerable<Shop> shopList)
        {
            if (!shopList.Any())
                return;
            for (var index = 0; index < Route.Count; index++)
            {
                var shop1 = Route[index];
                Shop closestShop = null;
                short closestDistance = 0;
                foreach (var shop2 in shopList.Where(x => x.Id != shop1.Id).ToList())
                {
                    if (shop2.Products > 0 || shop2.Chemistry > 0 || shop2.Drinks > 0)
                    {
                        var distance = (short)(Math.Abs(shop1.X - shop2.X) + Math.Abs(shop1.Y - shop2.Y));
                        if (closestDistance == 0 || closestDistance > distance)
                        {
                            closestDistance = distance;
                            closestShop = shop2;
                        }
                    }
                }

                if (!Overworking && !WillOverwork(closestDistance))
                {
                    Route.Add(closestShop);
                    var products = Products > closestShop.Products
                        ? closestShop.Products
                        : Products;
                    var chemistry = Chemistry > closestShop.Chemistry
                        ? closestShop.Chemistry
                        : Chemistry;
                    var drinks = Drinks > closestShop.Drinks
                        ? closestShop.Drinks
                        : Drinks;
                    Unload(products, chemistry, drinks);
                    closestShop.Upload(products, chemistry, drinks);
                }

                if (AnyEmpty() && !Overworking && shopList.Any(x => !x.AnyEmpty()))
                {
                    Route.Add(Route.First());
                    if (!Overworking)
                        Upload();
                }

                if (Overworking || shopList.All(x => x.AllEmpty()))
                {
                    if (Route.Last().Id != Route.First().Id)
                        Route.Add(Route.First());
                    break;
                }
            }
        }
    }

    public enum TransportType
    {
        Own,
        Hired
    }
}