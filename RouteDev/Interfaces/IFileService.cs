using RouteDev.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RouteDev.Interfaces
{
    public interface IFileService
    {
        List<Shop> Open(string filename);
        void Save(string filename, List<Shop> shopList, List<Transport> transportList);
    }
}
