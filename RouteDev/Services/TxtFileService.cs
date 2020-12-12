using RouteDev.Data;
using RouteDev.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace RouteDev.Services
{
    public class TxtFileService : IFileService
    {
        public List<Shop> Open(string filename)
        {
            var shops = new List<Shop>();

            using (var sr = new StreamReader(filename))
            {
                var content = sr.ReadToEnd();
                var text = content.Replace('-', '0').Split('\n');
                foreach (var line in text)
                {
                    var fields = line.Split('\t');

                    shops.Add(new Shop(
                        uint.Parse(fields[0]),
                        byte.Parse(fields[1]),
                        byte.Parse(fields[2]))
                    {
                        Products  = int.Parse(fields[3]),
                        Chemistry = int.Parse(fields[4]),
                        Drinks    = int.Parse(fields[5])
                    });
                }
            }

            return shops;
        }

        public void Save(string filename, List<Shop> shopList, List<Transport> transportList)
        {
            throw new System.NotImplementedException();
        }
    }
}