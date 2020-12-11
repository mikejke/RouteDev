using RouteDev.Data;
using System;
using System.Collections.Generic;
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
            //shop is close to main storage, which coordinates is 17 , 4
            var closeShop = new Shop(1, 17, 8)
            {
                Drinks = 20,
                Chemistry = 20,
                Products = 20
            };
            var farShop = new Shop(2, 12, 12)
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

            var distance = (short)(Math.Abs(17 - 20) + Math.Abs(4 - 10));

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

            var distance = (short)(Math.Abs(17 - 20) + Math.Abs(4 - 10));

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
            Assert.Equal(199, moqCar.Expenses);
        }
    }
}
