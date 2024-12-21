using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using StackExchange.Redis;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IConnectionMultiplexer _redis;

    public OrderController(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    [HttpPost("place")]
    public async Task<IActionResult> PlaceOrder([FromBody] OrderModel model)
    {
        var subscriber = _redis.GetSubscriber();

        var message = $"productId:{model.ProductId},quantity:{model.Quantity}";
        await subscriber.PublishAsync("order-placed", message);

        return Ok("Order placed and event published.");
    }
}
