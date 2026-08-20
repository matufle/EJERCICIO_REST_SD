var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();      // <- movido ACÁ, antes de Build()
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();                    // <- esto ya estaba bien ubicado

app.Run();