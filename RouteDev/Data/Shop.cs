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
    }
}