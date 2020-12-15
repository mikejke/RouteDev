using System;
using System.Collections.Generic;
using System.Linq;
using RouteDev.Utils;

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

        public List<Shop> Route { get; set; } = new List<Shop>() { Constants.Storage };

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

        //Переменная хранит издержки, если кол-во груза при загрузке меньше 90
        private double _tmpExpenses = 0;
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
                expenses += _tmpExpenses;

                return expenses + FixedExpenses;
            }
        }

        public bool Overworking => WorkingHours > AvgWorkTime;
        

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
        
        public void Upload(int p, int c, int d)
        {
            Products = p;
            Chemistry = c;
            Drinks = d;

            if(p + c + d < 90)
                _tmpExpenses += 2 * (90 - p + c + d);

            TotalUploadingTime += 0.5;
        }

        public short Distance 
        {
            get
            {
                short distance = 0;
                for (var i = 0; i < Route.Count - 1; i++)
                {
                    distance += Route[i].CalculateDistance(Route[i + 1]);
                }
                return distance;
            }
        }

        public bool WillOverwork(double distance) => (distance * 3 / 60) + WorkingHours > AvgWorkTime;
        public bool WillOverwork(double distance1, double distance2 ) => ((distance1 + distance2) * 3 / 60) + WorkingHours >= MaxWorkTime;

        public void CalculateRoutes(IEnumerable<Shop> shopList)
        {
            shopList = shopList.ToList();
            if (!shopList.Any())
                return;
            for (var index = 0; index < Route.Count; index++)
            {
                var shop = Route[index];

                var closestShop = shopList
                    .Where(s => s != shop &&
                                s.AnyNeed() &&
                                !WillOverwork(shop.CalculateDistance(s)) &&
                                !WillOverwork(s.CalculateDistance(Constants.Storage), s.CalculateDistance(shop)))
                    .OrderBy(shop.CalculateDistance).FirstOrDefault();

                if (closestShop == null)
                {
                    if (shop != Constants.Storage)
                        Route.Add(Constants.Storage);
                    break;
                }

                if (AnyEmpty() && shopList.Any(x => !x.AnyEmpty()) || Overworking)
                {
                    Route.Add(Constants.Storage);
                    if (Overworking)
                        break;
                    Upload();
                    continue;
                }

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
        }
    }

    public enum TransportType
    {
        Own,
        Hired
    }
}