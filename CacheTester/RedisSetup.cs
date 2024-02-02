using CacheTester.Models;
using CacheTester.Service;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace CacheTester;

public interface IRedisSetup
{
    public IDatabase DB { get; set; }
}

public class RedisSetup : IRedisSetup, IHostedService
{
    private readonly IDataGeneration initial;
    private readonly ILogger<RedisSetup> _logger;

    public RedisSetup(ILogger<RedisSetup> logger,IDataGeneration initial)
    {
        this.initial = initial;
        _logger = logger;
    }
   
    public IDatabase DB { get; set; }
    
    private IDatabase SeedingData()
    {
        _logger.LogInformation("Start seeding data to Redis...");
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
        IDatabase db = redis.GetDatabase();
        
        SearchCommands ft = db.FT();
        JsonCommands json = db.JSON();
        
        ft.Create("myIndex", new FTCreateParams().On(IndexDataType.HASH)
                .Prefix("doc:"),
            new Schema()
                .AddTextField("fias", 5.0)
                .AddTextField("kladr")
                .AddTextField("okato")
                .AddTextField("oktmo"));
        
        
       OkatoResponse[] data = initial.GetData(100);

        int index = 0;
        foreach (OkatoResponse item in data)
        {
            db.HashSet($"doc:{++index}", new[]
            {
                new HashEntry("fias", item.Fias.ToString()),
                new HashEntry("kladr", item.Kladr),
                new HashEntry("okato", item.Okato),
                new HashEntry("oktmo", item.Oktmo)
            });
             _logger.LogInformation($"doc:{index} was added to db with data: {item.Fias}, {item.Kladr}, {item.Okato}, {item.Oktmo}");
        }
        _logger.LogInformation("Data seeding to Redis completed.");
        return db;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
            DB = SeedingData();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // nothing to do
    }
}