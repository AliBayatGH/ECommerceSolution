using CatalogService.Data;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    private readonly CatalogDbContext _context;

    public CatalogController(CatalogDbContext context)
    {
        _context = context;
    }

    [HttpPost("restock")]
    public async Task<IActionResult> RestockProduct([FromQuery] int productId, [FromQuery] int quantity)
    {
        var product = await _context.Products.FindAsync(productId);

        if (product == null) return NotFound();

        product.Stock += quantity;
        await _context.SaveChangesAsync();

        return Ok();
    }
}
