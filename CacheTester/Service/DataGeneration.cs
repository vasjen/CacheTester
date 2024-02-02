using Bogus;
using CacheTester.Models;

namespace CacheTester.Service;

public class DataGeneration : IDataGeneration
{
    public OkatoResponse[] GetData(int seed)
    {
        OkatoResponse[] data = new Faker<OkatoResponse>()
            .StrictMode(true)
            .UseSeed(seed)
            .RuleFor(u => u.Fias, f => f.Random.Guid())
            .RuleFor(u => u.Kladr, f => f.Random.UInt().ToString())
            .RuleFor(u => u.Okato, f => f.Random.Int().ToString())
            .RuleFor(u => u.Oktmo, f => f.Random.Int().ToString())
            .Generate(10000)
            .ToArray();

        return data;
    }
}

public interface IDataGeneration
{
    OkatoResponse[] GetData(int seed);
}