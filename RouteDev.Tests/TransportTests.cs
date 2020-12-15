using RouteDev.Data;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using RouteDev.Utils;
using Xunit;

namespace RouteDev.Tests
{
    public class TransportTests
    {
        [Fact]
        public void CorrectlyBuildingRoute_InputIs1Shop_ReturnRouteLengthIs3()
        {
            //Arrange
            byte x = 12;
            byte y = 12;

            var moqCar = new Transport(TransportType.Own)
            {
                Drinks = 40,
                Chemistry = 40,
                Products = 40
            };

            var moqShopList = new List<Shop>()
            {
                new Shop(1, x, y)
                {
                    Drinks = 40,
                    Chemistry = 40,
                    Products = 40
                }
            };

            //Act
            moqCar.CalculateRoutes(moqShopList);

            //Assert
            Assert.Equal(3, moqCar.Route.Count);
        }

        [Fact]
        public void CorrectlyFindsClosestShop_InputIsShopList_ReturnRouteWithClosestAtIndex1()
        {
            //Arrange
            var moqCar = new Transport(TransportType.Own)
            {
                Drinks = 40,
                Chemistry = 40,
                Products = 40
            };
            //shop is close to main storage, which coordinates are 17 , 14
            var closeShop = new Shop(1, 17, 12)
            {
                Drinks = 20,
                Chemistry = 20,
                Products = 20
            };
            var farShop = new Shop(2, 1, 1)
            {
                Drinks = 20,
                Chemistry = 20,
                Products = 20
            };

            var moqShopList = new List<Shop>()
            {
                closeShop, farShop
            };

            //Act
            moqCar.CalculateRoutes(moqShopList);

            //Assert
            Assert.Equal(closeShop, moqCar.Route[1]);
            //Assert.Equal(3, moqCar.Route.Count);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(20)]
        [InlineData(10)]
        public void CorrectlyUnloadsCargo(int expectedResult)
        {
            //Arrange
            var moqCar = new Transport(TransportType.Own)
            {
                Drinks = 40,
                Chemistry = 40,
                Products = 40
            };

            var moqShopList = new List<Shop>()
            {
                new Shop(1, 20, 10)
                {
                    Drinks = moqCar.Drinks - expectedResult,
                    Chemistry = moqCar.Chemistry - expectedResult,
                    Products = moqCar.Products - expectedResult
                }
            };

            //Act
            moqCar.CalculateRoutes(moqShopList);

            //Assert
            Assert.Equal(expectedResult, moqCar.Products);
            Assert.Equal(expectedResult, moqCar.Chemistry);
            Assert.Equal(expectedResult, moqCar.Drinks);
        }

        [Fact]
        public void CorrectlyCalculatesDistance()
        {
            //Arrange

            byte x = 20;
            byte y = 10;

            var distance = (short)(Math.Abs(Constants.StorageX - 20) + Math.Abs(Constants.StorageY - 10));

            var moqCar = new Transport(TransportType.Own)
            {
                Drinks = 40,
                Chemistry = 40,
                Products = 40
            };

            var moqShopList = new List<Shop>()
            {
                new Shop(1, x, y)
                {
                    Drinks = 40,
                    Chemistry = 40,
                    Products = 40
                }
            };

            //Act
            moqCar.CalculateRoutes(moqShopList);

            //Assert
            Assert.Equal(distance * 2, moqCar.Distance);
        }

        [Fact]
        public void CorrectlyCalculatesExpenses()
        {
            //Arrange
            byte x = 20;
            byte y = 10;

            var distance = (short)(Math.Abs(Constants.StorageX - 20) + Math.Abs(Constants.StorageY - 10));

            var moqCar = new Transport(TransportType.Own)
            {
                Drinks = 40,
                Chemistry = 40,
                Products = 40
            };

            var moqShopList = new List<Shop>()
            {
                new Shop(1, x, y)
                {
                    Drinks = 40,
                    Chemistry = 40,
                    Products = 40
                }
            };

            //Act
            moqCar.CalculateRoutes(moqShopList);

            //Assert
            Assert.Equal(27, moqCar.Expenses);
        }

        [Fact]
        public void SendsCarOnceToAFarShop()
        {
            //Arrange
            byte x = 2;
            byte y = 88;

            var moqCar = new Transport(TransportType.Own)
            {
                Drinks = 40,
                Chemistry = 40,
                Products = 40
            };

            var moqShopList = new List<Shop>()
            {
                new Shop(1, x, y)
                {
                    Drinks = 80,
                    Chemistry = 80,
                    Products = 80
                }
            };

            //Act
            moqCar.CalculateRoutes(moqShopList);

            //Assert
            Assert.Equal(3, moqCar.Route.Count);
            Assert.True(moqCar.WorkingHours < 11);
        }

        [Fact]
        public void Sends2CarsOnceToAFarShop()
        {
            //Arrange
            byte x = 2;
            byte y = 80;

            var moqCars = new List<Transport>()
            {
                new Transport(TransportType.Own)
                {
                    Drinks = 40,
                    Chemistry = 40,
                    Products = 40
                },
                new Transport(TransportType.Own)
                {
                    Drinks = 40,
                    Chemistry = 40,
                    Products = 40
                },
            };

            var moqShopList = new List<Shop>()
            {
                new Shop(1, x, y)
                {
                    Drinks = 80,
                    Chemistry = 80,
                    Products = 80
                }
            };

            //Act
            moqCars.ForEach(car => car.CalculateRoutes(moqShopList));

            //Assert
            Assert.Equal(3, moqCars[0].Route.Count);
            Assert.Equal(3, moqCars[1].Route.Count);
            Assert.True(moqCars.All(car => car.WorkingHours < 11)); //none of cars is overworking
        }

        [Fact]
        public void SendsCarOnceToAFarShop_FromListOf2()
        {
            //Arrange
            var moqCar = new Transport(TransportType.Own)
            {
                Drinks = 40,
                Chemistry = 40,
                Products = 40
            };

            var moqShopList = new List<Shop>()
            {
                new Shop(1, 14, 88)
                {
                    Drinks = 20,
                    Chemistry = 20,
                    Products = 20
                },
                new Shop(1, 88, 14)
                {
                Drinks = 20,
                Chemistry = 20,
                Products = 20
                }
            };

            //Act
            moqCar.CalculateRoutes(moqShopList);

            //Assert
            Assert.Equal(3, moqCar.Route.Count);
            Assert.True(moqCar.WorkingHours < 11);
        }
    }
}
