using RouteDev.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RouteDev.Interfaces
{
    public interface IFileService
    {
        List<Shop> Open(string path);
        void Save(string path, List<Shop> shopList);
    }
}
