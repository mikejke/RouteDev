using RouteDev.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RouteDev
{
    /// <summary>
    /// Route Map draws map on a canvas with routes
    /// </summary>
    public partial class RouteMap : Window
    {
        private const byte Scale = 10;

        /// <summary>
        /// Constructor for RouteMap
        /// Gets list of shops and a list of cars with pre-calculated routes
        /// </summary>
        /// <param name="shopList"></param>
        /// <param name="cars"></param>
        public RouteMap(IEnumerable<Shop> shopList, List<Transport> cars)
        {
            InitializeComponent();

            foreach (var route in cars.Select(car => car.Route))
            {
                for (var i = 0; i < route.Count - 1; i++)
                {
                    DrawLineBetween(route[i], route[i + 1]);
                }
            }

            DrawPoints(shopList);
        }

        private void DrawLineBetween(Shop a, Shop b)
        {
            var line = new Line()
            {
                X1 = a.X * Scale,
                Y1 = a.Y * Scale,
                X2 = b.X * Scale,
                Y2 = b.Y * Scale,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            };
            Canvas.Children.Add(line);
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
                Width = Scale,
                Height = Scale,
                Fill = color ?? Brushes.Black,
            };
            Canvas.SetLeft(point, desiredX * Scale - (point.Width / 2));
            Canvas.SetTop(point, desiredY * Scale - (point.Height / 2));

            var id = new TextBlock()
            {
                Text = name,
                Foreground = Brushes.Black

            };
            Canvas.SetLeft(id, desiredX * Scale);
            Canvas.SetTop(id, desiredY * Scale - 20);

            Canvas.Children.Add(id);
            Canvas.Children.Add(point);
        }

        /// <summary>
        /// Converts canvas to .png image
        /// </summary>
        /// <returns></returns>
        public PngBitmapEncoder ToImage()
        {
            //actually, its never used, too bad
            var bmp = new RenderTargetBitmap((int)Canvas.ActualWidth, (int)Canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            Canvas.Measure(new Size((int)Canvas.ActualWidth, (int)Canvas.ActualHeight));
            Canvas.Arrange(new Rect(new Size((int)Canvas.ActualWidth, (int)Canvas.ActualHeight)));
            bmp.Render(Canvas);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            return encoder;
        }
    }
}
