using StackExchange.Redis;
using System.Text.Json;

namespace EventProcessingService;

public class EventProcessingWorker : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EventProcessingWorker> _logger;
    private readonly string _catalogServiceUrl;

    public EventProcessingWorker(IConnectionMultiplexer redis, IHttpClientFactory httpClientFactory, ILogger<EventProcessingWorker> logger, IConfiguration configuration)
    {
        _redis = redis;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _catalogServiceUrl = configuration["CatalogService:BaseUrl"];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redis.GetSubscriber();

        await subscriber.SubscribeAsync("order-placed", async (channel, message) =>
        {
            _logger.LogInformation("Received message on order-placed: {Message}", message);

            try
            {
                var order = JsonSerializer.Deserialize<OrderModel>(message.ToString());
                var httpClient = _httpClientFactory.CreateClient();

                var response = await httpClient.PostAsync(
                    $"{_catalogServiceUrl}/api/catalog/restock?productId={order.ProductId}&quantity={-order.Quantity}",
                    null);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully updated product inventory for ProductId: {ProductId}", order.ProductId);
                }
                else
                {
                    _logger.LogError("Failed to update inventory. StatusCode: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message on order-placed");
            }
        });

        _logger.LogInformation("EventProcessingWorker is listening for messages on order-placed.");
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

public class OrderModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
