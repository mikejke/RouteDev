using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using RouteDev.Data;
using RouteDev.Interfaces;

namespace RouteDev.Services
{
    public class TxtFileService : IFileService
    {
        public List<Shop> Open(string path)
        {
            var shops = new List<Shop>();

            using (var sr = new StreamReader(path))
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

        public void Save(string path, List<Shop> shopList)
        {
            //DataContractJsonSerializer jsonFormatter =
            //    new DataContractJsonSerializer(typeof(List<Shop>));
            //using (var fs = new FileStream(filename, FileMode.Create))
            //{
            //    jsonFormatter.WriteObject(fs, phonesList);
            //}
        }
    }
}