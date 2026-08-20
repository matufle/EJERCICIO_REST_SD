using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        // Simulamos una "base de datos" de stock en memoria.
        // static para que persista entre requests (mientras el proceso viva).
        private static readonly Dictionary<string, int> Stock = new()
        {
            ["P001"] = 100,
            ["P002"] = 50
        };

        [HttpGet("check/{productId}/{quantity}")]
        public IActionResult CheckStock(string productId, int quantity)
        {
            var available = Stock.TryGetValue(productId, out var stockQty) && stockQty >= quantity;
            return Ok(new { ProductId = productId, Available = available, CurrentStock = stockQty });
        }

        [HttpPost("order")]
        public IActionResult CreateOrder([FromBody] OrderRequest request)
        {
            if (Stock.TryGetValue(request.ProductId, out var stockQty) && stockQty >= request.Quantity)
            {
                Stock[request.ProductId] -= request.Quantity;
                return Ok(new { Success = true, Message = "Pedido confirmado, stock descontado." });
            }
            return BadRequest(new { Success = false, Message = "Stock insuficiente." });
        }
    }

    public class OrderRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}