using System;
using System.Threading.Tasks;
using MiniIndex.Models;

namespace MiniIndex.Core.Minis
{
    public interface IParser
    {
        bool CanParse(Uri url);
        Task<Mini> ParseFromUrl(Uri url);
    }
}