﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RouteDev.Data;

namespace RouteDev
{
    /// <summary>
    /// Логика взаимодействия для RouteMap.xaml
    /// </summary>
    public partial class RouteMap : Window
    {
        private const byte Size = 10;

        public RouteMap(IEnumerable<Shop> shopList, List<Transport> cars)
        {
            InitializeComponent();

            foreach (var route in cars.Select(car => car.Route))
            {
                for (var i = 1; i < route.Count; i++)
                {
                    var line = new Line()
                    {
                        X1 = route[i - 1].X * Size,
                        Y1 = route[i - 1].Y * Size,
                        X2 = route[i].X * Size,
                        Y2 = route[i].Y * Size,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                    };
                    Canvas.Children.Add(line);
                }
            }

            DrawPoints(shopList);
        }

        private void DrawPoints(IEnumerable<Shop> shopList)
        {
            CreatePoint("Storage", 17, 14, Brushes.ForestGreen);

            foreach (var shop in shopList)
            {
                CreatePoint(shop.Id.ToString(), shop.X, shop.Y);
            }
        }

        private void CreatePoint(string name, byte desiredX, byte desiredY, Brush color = null)
        {
            var point = new Ellipse()
            {
                Width = Size,
                Height = Size,
                Fill = color ?? Brushes.Black,
            };
            Canvas.SetLeft(point, desiredX * Size - (point.Width / 2));
            Canvas.SetTop(point, desiredY * Size - (point.Height / 2));

            var id = new TextBlock()
            {
                Text = name,
                Foreground = Brushes.Black

            };
            Canvas.SetLeft(id, desiredX * Size);
            Canvas.SetTop(id, desiredY * Size - 20);

            Canvas.Children.Add(id);
            Canvas.Children.Add(point);
        }
    }
}