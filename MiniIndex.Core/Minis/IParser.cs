using MiniIndex.Models;
using System;
using System.Threading.Tasks;

namespace MiniIndex.Core.Minis
{
    public interface IParser
    {
        string Site { get; }

        bool CanParse(Uri url);

        Task<Mini> ParseFromUrl(Uri url);
    }
}