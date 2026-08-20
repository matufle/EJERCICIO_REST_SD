using System.Net.Http.Json;

var client = new HttpClient();
client.BaseAddress = new Uri("http://inventoryservice:8080/");

string productId = "P001";
int quantity = 3;

Console.WriteLine($"[OrdersService] Consultando stock de {productId} (cantidad: {quantity})...");

// Reintentamos la conexión unas cuantas veces por si InventoryService
// todavía no terminó de levantar cuando arrancamos.
StockCheckResponse? checkResponse = null;
int maxRetries = 5;
for (int attempt = 1; attempt <= maxRetries; attempt++)
{
    try
    {
        checkResponse = await client.GetFromJsonAsync<StockCheckResponse>(
            $"api/inventory/check/{productId}/{quantity}");
        break; // si funcionó, salimos del loop
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"[OrdersService] Intento {attempt}/{maxRetries} falló: {ex.Message}");
        if (attempt == maxRetries) throw;
        await Task.Delay(2000); // esperamos 2 segundos antes de reintentar
    }
}

if (checkResponse is null)
{
    Console.WriteLine("[OrdersService] No se pudo consultar el stock.");
    return;
}

Console.WriteLine($"[OrdersService] Respuesta: Available={checkResponse.Available}, Stock actual={checkResponse.CurrentStock}");

if (checkResponse.Available)
{
    var orderResponse = await client.PostAsJsonAsync("api/inventory/order",
        new { ProductId = productId, Quantity = quantity });

    var result = await orderResponse.Content.ReadFromJsonAsync<OrderResult>();

    if (orderResponse.IsSuccessStatusCode)
        Console.WriteLine($"[OrdersService] Pedido confirmado: {result?.Message}");
    else
        Console.WriteLine($"[OrdersService] No se pudo confirmar el pedido: {result?.Message}");
}
else
{
    Console.WriteLine("[OrdersService] Stock insuficiente, no se genera el pedido.");
}

record StockCheckResponse(string ProductId, bool Available, int CurrentStock);
record OrderResult(bool Success, string Message);