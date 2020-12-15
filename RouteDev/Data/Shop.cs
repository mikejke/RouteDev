using System;

namespace RouteDev.Data
{
    public class Shop : Cargo
    {
        public uint Id { get; private set; }
        public byte X { get; private set; }
        public byte Y { get; private set; }

        public string Title => $"Магазин #{Id}";

        public string Coordinates => $"X: {X}, Y: {Y}";

        public Shop(uint id, byte x, byte y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public void Upload(int p, int c, int d)
        {
            Products -= p;
            Chemistry -= c;
            Drinks -= d;
        }

        public short CalculateDistance(Shop shop)
        {
            return (short)(Math.Abs(X - shop.X) + Math.Abs(Y - shop.Y));
        }

        public short CalculateDistance(uint x, uint y)
        {
            return (short)(Math.Abs(X - x) + Math.Abs(Y - y));
        }

        public bool AnyNeed() => Products != 0 || Chemistry != 0 || Drinks != 0;
    }
}